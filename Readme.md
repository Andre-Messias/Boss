Explorando a Interface entre genótipo e fenótipo com um modelo simples.

Alunos: Matheus Marques dos Santos (número usp: 12547170\) e André Luiz Santos Messias

Curso: Engenharia da Computação

Disciplina: Algorítimos Evolutivos Aplicados a Robótica

Professor Eduardo do Valle Simões

Tentamos explorar a diferença entre genótipo e fenótipo com um modelo simples. Nosso sistema consiste em quatro organismos, um boss e três caçadores, engajados em um confronto físico em um ambiente limitado e sem obstáculos. O objetivo do boss é matar todos os caçadores e seu comportamento é controlado por uma inteligência artificial simples já fornecida pela Unity. Os caçadores são cada um deles controlados pelo algorítimo evolutivo descrito a seguir.

Cada caçador é capaz de realizar sete ações distintas: andar para frente, andar para trás, andar para a direita, andar para a esquerda, rotacionar para a direita, rotacionar para a esquerda e atacar. Essas ações podem ser realizadas isoladamente ou concomitantemente. Portanto, definimos um gene no cromossomo de um caçador como um conjunto de sete floats com valor entre zero e um, cada um dos quais codifica a probabilidade de que ele realize uma das sete ações supracitadas. Por exemplo, se o float equivalente a andar para frente é 0.8 e o float equivalente a atacar é 0.5, então o caçador tem uma probabilidade de andar para frente de 80% e uma probabilidade de atacar de 50% quando esse gene for executado. A probabilidade de que o caçador ande para frente e ataque ao mesmo tempo seria de 0.8\*0.5 \= 0.4 \= 40%. Vale ressaltar que ações opostas se cancelam. Assim, se um caçador decide andar para a esquerda e para a direita ao mesmo tempo, ele fica parado na direção direita-esquerda.

A cada tick de uma dada rodada de treinamento, um dos genes no cromossomo de cada caçador é executado de acordo com o seguinte processo. Para cada um dos sete locus no gene, é sorteado um float entre zero e um. Se esse float é menor do que o float contido naquele locus, a ação correspondente ao locus é executada. Caso contrário, a ação não é executada. Assim, se tivermos o gene abaixo:

| Andar para frente | Andar para trás | Andar para a esquerda | Andar para a direita | Rotacionar para a direita | Rotacionar para a esquerda | Atacar |
| :---- | :---- | :---- | :---- | :---- | :---- | :---- |
| 0.5 | 0.8 | 0.6 | 0.1 | 0.72 | 0.96 | 0.02 |

Executamos-o sorteando sete floats, a exemplo:

| 0.1 | 0.8 | 0.9 | 0.5 | 0.6 | 0.99 | 0.1 |
| :---- | :---- | :---- | :---- | :---- | :---- | :---- |

Nesse caso, o caçador andaria para a frente e rotacionaria para a direita durante esse tick do relógio.

O processo de cruzamento de dois genes acontece somando-se os coeficientes de cada locus e dividindo-os por dois. Assim, se um gene com probabilidade de 50% de andar para frente é cruzado com um com probabilidade de 80% de andar para a frente, o resultado será um gene com probabilidade de (50%+80%)/2 \= 65% de andar para frente. Uma consequência interessante desse método de cruzamento é que cruzar dois genes idênticos gera um gene igual aos dos dois pais.

Os genes são executados sequencialmente, um a cada tick. Após executar o último gene do cromossomo, o primeiro é executado novamente. Como o sistema tem um comportamento probabilístico, essa nova execução do primeiro gene tende a resultar em um comportamento diferente da primeira execução.

Uma rodada de treinamento acaba quando ou os três caçadores ou o boss morre. Após a rodada, todos os caçadores possuem uma nota, a qual foi determinada com base nos seguintes parâmetros: morte do boss (aumenta a nota), morte dos caçadores (diminui a nota), quantidade de ataques sofridos pelos caçadores (diminui a nota), quantidade de ataques sofridos pelo boss (aumenta a nota), tempo necessário para terminar a rodada (diminui a nota). Definimos a nota de uma gaiola como a soma das notas dos caçadores nessa gaiola.

Fazendo várias gaiolas com um boss e três caçadores em cada uma delas, deixamos as rodadas acontecerem em cada uma das gaiolas. Quando todas as gaiolas terminam uma rodada, comparamos as notas e cruzamos a melhor gaiola com todas as outras, mantendo a melhor intacta. O cruzamento acontece caçador a caçador. Ou seja, o caçador um da melhor gaiola cruza com o caçador um de cada uma das demais gaiolas, o caçador dois da melhor gaiola cruza com o caçador dois de cada uma das demais gaiolas e o caçador três da melhor gaiola cruza com o caçador três de cada uma das demais gaiolas.

No entanto, da forma que o programa está atualmente escrito, observamos que a convergência das gerações para algum tipo de comportamento inteligente é muito lenta no melhor dos casos e nem chega a ocorrer no pior deles. Observado esse problema, desenvolve-mos algumas hipóteses para explica-lo:

1 – O algorítimo evolutivo implementado é muito probabilístico: Talvez o fato de a probabilidade de cada comportamento variar de 0 a 100% gere um comportamento muito caótico no sistema, dificultando-o de encontrar ótimos locais. A solução seria restringir os locus em cada gene para variar entre um valor basal maior do 0.0 e um valor máximo menor que 1.0. Poderíamos, inclusive, evoluir essa faixa para encontrar o melhor intervalo possível para que o sistema convirja mais rapidamente para um comportamento inteligente.

2 – Estamos jogando fora o melhor de todos: Da forma escrita, avaliamos as notas das gaiolas ao final das rodadas e deixamos a melhor gaiola transar com todas as outras. No entanto,  como o comportamento de uma gaiola é probabilístico, se deixassemos as mesmas gaiolas passarem por uma segunda rodada, a distribuição de notas poderia ser diferente. Por exemplo, digamos que a nota mínima que a gaiola um é capaz de tirar (com base no makeup genético de seus caçadores) é 20, mas que a nota máxima dessa gaiola é 80\. Se estamos em uma rodada em que a gaiola um tirou 20, não devíamos tê-la descartado tão rápido. A solução para isso seria apenas cruzar as gaiolas após deixa-las rodar por várias gerações. A nota de cada gaiola seria a média das suas notas nas gerações que se passaram.

3 – O modelo é muito simples: Da forma como o programa está escrito, a expressão do fenótipo (comportamento) a partir do genótipo (cromossomo) é aleatória. No entanto, na natureza, a expressão de um fenótipo a partir do seu genótipo está longe de ser aleatória. Fatores ambientais determinam as probabilidades de que cada gene se expresse em determinado instante do tempo. Talvez o modelo usado, em não capturar essa característica do mundo real, fique muito caótico. A solução seria alterar a forma como o array de floats que determina a execução de um gene é sorteada com base no contexto do caçador. Por exemplo, se um caçador está longe do boss, não há motivo para que ele ataque. Portanto, gostaríamos que o float sorteado para comparar com sua probabilidade genética de ataque fosse próximo de um. No entanto, quando o mesmo caçador estiver próximo do boss, gostaríamos que esse mesmo float fosse mais próximo de zero. A solução é enviezar nosso gerador de números aleatórios com base na distância que o caçador está do boss.

Abaixo, está um vídeo explicativo sobre o programa mostrando o processo de evolução, algumas rodadas e a interface:

[https://drive.google.com/file/d/1yJuMYfASd2SYxsLbHhu2XsrLTnfvGNN6/view?usp=sharing](https://drive.google.com/file/d/1yJuMYfASd2SYxsLbHhu2XsrLTnfvGNN6/view?usp=sharing)