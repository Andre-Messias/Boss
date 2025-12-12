using NUnit.Framework;
using UnityEngine;

public class HunterBrain : MonoBehaviour
{
    #region Classes
    // Gene for a single action
    [System.Serializable]
    public class Gene
    {
        public float moveLeft;
        public float moveRight;
        public float moveForward;
        public float moveBackward;

        public float rotateLeft;
        public float rotateRight;

        public float attack;

        public Gene()
        {
            moveLeft = 0;
            moveRight = 0;
            moveForward = 0;
            moveBackward = 0;

            rotateLeft = 0;
            rotateRight = 0;

            attack = 0;
        }

        // Crossover constructor
        public Gene(Gene father, Gene mother)
        {
            moveLeft = (father.moveLeft + mother.moveLeft) / 2;
            moveRight = (father.moveRight + mother.moveRight) / 2;
            moveForward = (father.moveForward + mother.moveForward) / 2;
            moveBackward = (father.moveBackward + mother.moveBackward) / 2;

            rotateLeft = (father.rotateLeft + mother.rotateLeft) / 2;
            rotateRight = (father.rotateRight + mother.rotateRight) / 2;

            attack = (father.attack + mother.attack) / 2;
        }

        // Copy constructor
        public Gene(Gene other)
        {
            moveLeft = other.moveLeft;
            moveRight = other.moveRight;
            moveForward = other.moveForward;
            moveBackward = other.moveBackward;
            rotateLeft = other.rotateLeft;
            rotateRight = other.rotateRight;
            attack = other.attack;
        }

        // Get random float array for gene actions
        public static float[] GetRandom()
        {
            float[] random = new float[7];
            for (int i = 0; i < 7; i++)
            {
                random[i] = Random.Range(0f, 1f);
            }

            return random;
        }

        // Generate a random gene
        public static Gene RandomGene()
        {
            Gene gene = new();
            float[] random = GetRandom();
            gene.moveLeft = random[0];
            gene.moveRight = random[1];
            gene.moveForward = random[2];
            gene.moveBackward = random[3];

            gene.rotateLeft = random[4];
            gene.rotateRight = random[5];

            gene.attack = random[6];
            return gene;
        }
    }

    // Chromosome consisting of multiple genes
    [System.Serializable]
    public class Chromosome
    {
        public Gene[] genes;
        public int index;
        public int grade;

        public Chromosome(int geneCount)
        {
            genes = new Gene[geneCount];
            for (int i = 0; i < geneCount; i++)
            {
                genes[i] = new Gene();
            }
        }

        // Crossover constructor
        public Chromosome(Chromosome father, Chromosome mother)
        {
            int geneCount = Mathf.Min(father.genes.Length, mother.genes.Length);
            genes = new Gene[geneCount];
            for (int i = 0; i < geneCount; i++)
            {
                genes[i] = new Gene(father.genes[i], mother.genes[i]);
            }
        }

        // Copy constructor
        public Chromosome(Chromosome other)
        {
            genes = new Gene[other.genes.Length];
            for(int i = 0; i < other.genes.Length; i++)
            {
                genes[i] = new Gene(other.genes[i]);
            }
            grade = other.grade;
        }

        // Generate a random chromosome
        public static Chromosome RandomChromosome(int geneCount)
        {
            Chromosome chromosome = new Chromosome(geneCount);
            for (int i = 0; i < geneCount; i++)
            {
                chromosome.genes[i] = Gene.RandomGene();
            }
            return chromosome;
        }

        // Get the next gene in the chromosome
        public Gene GetNextGene()
        {
            Gene gene = genes[index];
            index = (index + 1) % genes.Length;
            return gene;
        }

        // Reset the chromosome index and grade
        public void Reset()
        {
            index = 0;
            grade = 0;
        }
    }

    #endregion

    [Header("Components")]
    [SerializeField] private Hunter _hunter;

    [Header("Brain")]
    [SerializeField] private float _actionInterval = 0.1f;
    [SerializeField] private float _actionTimer = 0f;
    [SerializeField] private float _gameDuration = 600f;
    public Chromosome chromosome;
    [SerializeField] private bool _regenerateChromosome = true;

    private void Awake()
    {
        if (_hunter == null && !TryGetComponent(out _hunter))
        {
            Debug.LogError("Hunter component is missing on AG GameObject.", this);
        }
        else
        {
            _hunter.OnHitMonster += OnHitMonster;
            _hunter.OnHunterHit += OnHit;
        }

        if (_regenerateChromosome || chromosome == null)
        {
            chromosome = Chromosome.RandomChromosome((int)(_gameDuration / _actionInterval));
        }
    }

    private void OnDestroy()
    {
        _hunter.OnHunterHit -= OnHit;
        _hunter.OnHitMonster -= OnHitMonster;
    }

    private float _timeGrade = 0f;
    private void Update()
    {
        // Inactive check
        if (!_hunter.isAlive || !_hunter.active) return;

        // Time grade
        _timeGrade += Time.deltaTime;
        if(_timeGrade >= 1f)
        {
            _timeGrade = 0f;
            chromosome.grade -= 1;
        }

        // Gene action
        _actionTimer += Time.deltaTime;
        if(_actionTimer >= _actionInterval)
        {
            _actionTimer = 0f;
            Gene gene = chromosome.GetNextGene();
            UseGene(gene);
        }
    }

    // Event handlers
    public void OnHitMonster()
    {
        chromosome.grade += 20;
    }

    public void OnHit(Hunter hunter)
    {
        chromosome.grade -= 8;
    }

    // Use a gene to control the hunter
    public void UseGene(Gene gene)
    {
        float[] chances = Gene.GetRandom();
        Vector2 moveDir = Vector2.zero;
        if(chances[0] < gene.moveLeft)
        {
            moveDir += Vector2.left;
        }
        if (chances[1] < gene.moveRight)
        {
            moveDir += Vector2.right;
        }
        if (chances[2] < gene.moveForward)
        {
            moveDir += Vector2.up;
        }
        if (chances[3] < gene.moveBackward)
        {
            moveDir += Vector2.down;
        }
        _hunter.MoveDir(moveDir.normalized);

        float rotateDir = 0f;
        if (chances[4] < gene.rotateLeft)
        {
            rotateDir -= 1f;
        }
        if (chances[5] < gene.rotateRight)
        {
            rotateDir += 1f;
        }
        _hunter.RotateDir(rotateDir);

        if (chances[6] < gene.attack)
        {
            _hunter.Attack();
        }
    }
}
