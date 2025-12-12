using System;
using System.Collections.Generic;
using UnityEngine;

public class AG : MonoBehaviour
{
    // Save logs for boss kills
    [Serializable]
    struct BossLog
    {
        public int generation;
        public int bossKills;
    }

    [Min(0)]
    [SerializeField] private float _timeScale = 1f;

    [Header("Brain")]
    [SerializeField] private string _cageTag = "Cage";
    [SerializeField] private List<Cage> _cages = new();
    [SerializeField] private HunterBrain.Chromosome[] _bestChomosomes = new HunterBrain.Chromosome[3];
    [SerializeField] private HunterBrain.Chromosome[] _historyBestChomosomes = new HunterBrain.Chromosome[3];
    [SerializeField] private int _historyBestGrade = 0;

    [Header("Logs")]
    [SerializeField] private List<BossLog> _bossLogs = new();
    [SerializeField] private int _generation = 0;

    private void Awake()
    {
        // Find cages
        GameObject[] cageObjects = GameObject.FindGameObjectsWithTag(_cageTag);
        foreach(var cageObject in cageObjects)
        {
            if(cageObject.TryGetComponent(out Cage cage))
            {
                _cages.Add(cage);
            }
        }
    }

    void Update()
    {
        // Update time scale
        Time.timeScale = _timeScale;

        // Check cages
        foreach(var cage in _cages)
        {
            if (!cage.finished)
            {
                return;
            }
        }

        // Merge chromosomes

        Cage bestCage = _cages[0];
        int bestGrade = bestCage.GetGrade();
        int bossKills = 0;
        foreach (var cage in _cages)
        {
            if (!cage.monster.isAlive)
            {
                bossKills++;
            }
            if(bestGrade < cage.GetGrade())
            {
                bestGrade = cage.GetGrade();
                bestCage = cage;
            }
        }
        _bestChomosomes[0] = new(bestCage._hunters[0].GetComponent<HunterBrain>().chromosome);
        _bestChomosomes[1] = new(bestCage._hunters[1].GetComponent<HunterBrain>().chromosome);
        _bestChomosomes[2] = new(bestCage._hunters[2].GetComponent<HunterBrain>().chromosome);

        // Log boss kills
        BossLog log = new()
        {
            generation = _generation,
            bossKills = bossKills
        };
        _bossLogs.Add(log);
        _generation++;

        foreach (var cage in _cages)
        {
            // C1
            HunterBrain.Chromosome chromosome1 = cage._hunters[0].GetComponent<HunterBrain>().chromosome;
            chromosome1 = new(chromosome1, _bestChomosomes[0]);

            // C2
            HunterBrain.Chromosome chromosome2 = cage._hunters[1].GetComponent<HunterBrain>().chromosome;
            chromosome2 = new(chromosome2, _bestChomosomes[1]);

            // C3
            HunterBrain.Chromosome chromosome3 = cage._hunters[2].GetComponent<HunterBrain>().chromosome;
            chromosome3 = new(chromosome3, _bestChomosomes[2]);

        }

        // Save history best chromosomes
        if (bestGrade > _historyBestGrade)
        {
            _historyBestGrade = bestGrade;
            _historyBestChomosomes[0] = new(_bestChomosomes[0]);
            _historyBestChomosomes[1] = new(_bestChomosomes[1]);
            _historyBestChomosomes[2] = new(_bestChomosomes[2]);
        }

        // Reset cages
        foreach (var cage in _cages)
        {
            cage.ResetCage();
        }
    }
}
