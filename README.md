
<!-- PROJECT LOGO -->
<br />
<p align="center">
  <h3 align="center">Ultimate Chess</h3>

  <p align="center">
    Chess Game in Unity with Machine Learning
    <br />
    <a href="https://github.com/rufimelo99/UltimateChess"><strong>Check Files »</strong></a>
    <br />
    <br />
    · This project was done in Artificial Intelligence for Games at 
    <a href="https://tecnico.ulisboa.pt/en/">IST</a>
  </p>
</p>

<!-- TABLE OF CONTENTS -->
<details open="open">
  <summary>Table of Contents</summary>
  <ol>
    <li>
      <a href="#about-the-project">About The Project</a>
      <ul>
        <li><a href="#built-with">Built With</a></li>
      </ul>
    </li>
    <li>
      <a href="#getting-started">Getting Started</a>
      <ul>
        <li><a href="#prerequisites">Prerequisites</a></li>
      </ul>
    </li>
    <li><a href="#brief-chess-introduction">Brief Chess Introduction</a></li>
    <li><a href="#usage">Usage </a></li>
    <li><a href="#rewards-system">Rewards System</a></li>
    <li><a href="#improvements">Improvements</a></li>
    <li><a href="#training-the-agent">Training The Agent</a></li>
    <li><a href="#acknowledgements">Acknowledgements</a></li>
  </ol>
</details>



<!-- ABOUT THE PROJECT -->
## About The Project

This Project has been made in accordance to the generic guidelines for the IAJ's - Inteligência Artificial para Jogos (Artificial Intelligence for Games) fourth project.

The fourth project focus in the use of Unity’s Deep Reinforcement Learning SDK and its application in a game-like scenario to make an agent learn how to play a game. 
Knowing this, the goal became building a Chess Game and create an agent that would learn how to play that same game using the Unity Machine Learning Agents Toolkit (_ML_-_Agents_).

### Built With
* [Unity](https://unity.com)
* [ML-Agents](https://github.com/Unity-Technologies/ml-agents)
* [Anaconda](https://www.anaconda.com)


### Prerequisites
Unity is required to run this project, obviously and so is the installation of the toolkit.
For preferential use, an environment was created using Anaconda for training purposes and later on tensorboard for tracking the learning process.

`tensorboard --logdir=results --port 6006`

<!-- USAGE EXAMPLES -->
## Brief Chess Introduction
Chess is an ancient board game for two players composed by turns. Each player has its own color, either white or black. The game starts with white's turn. During each turn, the respective player can make one movement and, after that, the turn is passed to the opponent.
There are 6 different pieces types: Rook or Tower, Horse or Knight, Bishop, Queen, King and Pawn. Each different type can make different movements and the ultimate goal is to eat the opponent King (through checkmate, but not in this project).
The Rook can move in straight lines while the Bishop can move in the diagonals. The Queen is a mixed of both Rook and Bishop. The Knight is special, since it is the only type of piece capable of jumping over pieces. It moves in "L" shape movements (3 units in a straight line plus 1 in a straight line orthogonal to the first 3 units line). The King can move towards one tile in either direction. Finally, the Pawn can move 1 unit forward or 2 if it is that piece first movement. Additionally, it can eat an enemy piece within one unit in one of the diagonals in front, taking its place.
In order to address some special movements, there exists 3 of them. The first one is Castling which basically lets the King swap with a Rook if the King is not threatened and both the Rook and the King did not move previously. The second one is Converting, which is when a Pawn reach the opposite side of the board converting into a more powerful piece (since the queen is the most powerful piece, it converts into a Queen). The last one is En Passant. En Passant is a little more tricky. It occurs only when an opponent Pawn makes the 2 units movement and there is a players Pawn who could eat that piece if it only moved 1 unit. The players Pawn can proceed to eat the Piece in the diagonal, even thought the enemy piece is not in that position.


<!-- USAGE EXAMPLES -->
## Usage
The Project has a simple menu that allows the user to choose to either play against another user on the same computer. The game proceeds by simply clicking on a tile and select where to move that piece. Some highlights are shown over the tiles to help the user know where that same piece can, in fact, move. When is desired to switch the neural network, it is done by simply swapping the existing neural network from the agents to the one intended.
![simple menu](https://user-images.githubusercontent.com/44201826/103315976-07309c80-4a1f-11eb-99cf-dfe677c6378e.PNG)

![UltimateCHess2](https://user-images.githubusercontent.com/44201826/102122430-5e3e4b00-3e3d-11eb-9814-3c8ebdeb32f3.PNG)

<!-- USAGE EXAMPLES -->
## Rewards System
In order to allow the agent to learn sequentially, it needs to receive some rewards and adjust its actions according to the observations made.
Firstly, it will be explained briefly how the learning process works.
Shortly, the agent has episodes and, in each episode it will perform a certain number of observations (all the positions of the pieces) and it will try to generate an action.
Knowing that the number of observations made needs to be always the same, it was decided that the agent will receive the positions of all the pieces on the board ( it would be -1 in case of non existence) plus 8 possible queens for each player. These extra queens represent queens that can show up when a pawn reaches a tile on the opposite side of the board, converting, summing up to **97** observations each time (including one Boolean variable that represents the color of the pieces which the agent is playing with). 
When it comes to generating an action, at the position `0` of the `vectorAction` contains one of all the possible moves that could appear.
Also, since the size of `vectorAction[0]` must be always the same, this means that, for instance, for a Pawn, there must be always 4 possibilities "stored" in that `vectorAction` position (even if it can only perform one of those movements). All summed up, the result is **676** different possibilities for actions.
Since the number of actions that could be generated was too big, the number of extra queens were reduced from 8 in each side to 3 (which is a more realistic approach). This way, the number of observations is **77**  and the number of possible actions is **394**. This approach intends to speed up the learning process.

The goal is to make the agent learn by playing against itself. This is done through ml-agents `selfplay()`. This means that each agent will be playing against a snapshot of itself (with a fixed policy). Knowing this, one way to know if the agent learn is through the [ELO](https://en.wikipedia.org/wiki/Elo_rating_system), which will represent the skill level of the agent. This skill rate is updated in case off victory or loss when facing the older snapshot.
It is important to note that by changing the snapshot which is playing against, it reduces the probably of overfitting. 

Going into more detail, the agent needs to know if a certain action is doable or not and, ideally, distinguished between possible movements. Some are better than others even though they are possible. 

The first step is to evaluate it a certain movement is valid or not, through checking that piece `PossibleMove(BoardManager instance)`, and give an according reward. In case of choosing an invalid move, it is given the `invalidAction_or_doNothing` reward which its value can be changed in the Inspector. On the other hand, if the move is valid, it receives the `validAction` reward. This process is done by, first checking which Piece the agent is trying to move though the number of `vectorAction[0]` and then seeing if that particular movement is valid by using the Behavior functions. 

For instance, if `vectorAction[0]` has a value of 1 while a King is on *a8*, representing that the King should move one tile up and one tile to the left, it is invalid and, consequentially, should penalize the agent. Note that since the size of `vectorAction[0]` needs to be always the same, it is needed to penalize the agent when it tries to access an inexistent piece.

Ultimately, it receives a larger reward depending if it wins or loses:
`wonGame` and `lostGame`. According to the documentation, it should be:
`public float wonGame = 1.0f;`
`public float lostGame = -1.0f;`

In general, eating an opposite piece should be good helpful and eating a Pawn is different from eating a Queen. Therefore, strengths were added:

`public float strengthPawn        = 0.0001f;`
`public float strengthHorse       = 0.0003f;`
`public float strengthBishop      = 0.0003f;`
`public float strengthRook        = 0.0005f;`
`public float strengthQueen       = 0.0009f;`
`public float strengthKing        = 0.005f;`

Those strengths were set according to a [relative value](https://en.wikipedia.org/wiki/Chess_piece_relative_value) of each piece, but can be changed in the Inspector. When eating a certain piece, the agent receives the according strength times 10.

## Improvements
With the goal of optimizing the learning experience, some extra rewards systems were added.
The reward `incentiveToCastling` with the value of `0.1f` intends to incentivize the agent to perform the castling movement which is a very powerful movement. On the same note, since converting a Pawn into a Queen is really good, `incentiveToConverting` was added.
After a few hundred thousand simulations, the agent already "develops" the knights/horses more frequently, but it also advances the king further in the map. In order to try to control this phenomenon and improve the overall positioning and valorization of the pieces, some bidimensional arrays were added that, combined with a relative value of each piece, allow the agent to learn more correctly. 
These bidimensional arrays basically give a value for each position of a certain piece on the board. For instance, as referred before, if there is a king on the opposite side of the board, it would be really bad for that player. On the other hand, if it was on the own side of the board, it should be better. 
**Example:**
| |  |  |  |	|  |  |  |
|--|--| -- |--  |--|--|--| -- |
| -3.0 | -4.0 | -4.0 |-5.0  |-5.0| -4.0 | -4.0 |-3.0  |
| -3.0 | -4.0 | -4.0 |-5.0  |-5.0| -4.0 | -4.0 |-3.0  |
| -3.0 | -4.0 | -4.0 |-5.0  |-5.0| -4.0 | -4.0 |-3.0  |
| -3.0 | -4.0 | -4.0 |-5.0  |-5.0| -4.0 | -4.0 |-3.0  |
| -2.0 | -3.0 | -3.0 |-2.0  |-4.0| -3.0 | -3.0 |-2.0  |
| -1.0 | -2.0 | -2.0 |-2.0  |-2.0| -2.0 | -2.0 |-1.0  |
| 2.0 | 0.0 |  0.0 |  0.0 |  0.0 |  0.0 |  0.0 |  2.0 | 
| 2.0 | 3.0 |  0.0 |  0.0 |  0.0 |  0.0 |  3.0 |  2.0 | 


*(King Piece's Table)* 
Saying that the piece exists and the move is valid, the agent will be rewarded with the `validAction` reward and a small increment or decrement related with the positioning of that same piece in the board using the bidimensional arrays. For instance, the King goes from c1 to b1 (the letter represents the column and the number describes the row)
, it should receive the `validAction` reward plus `3.0` x `strengthKing`.

Finally, some custom initial game disposition was added. Those custom disposition include a scenario with only kings and rooks or only kings and pawn. The idea in mind was to help the agent to learn the possible combinations of each type of piece. 

<!-- USAGE EXAMPLES -->
## Training the Agent

In order to train the agent, it is necessary to change the scene in the Scene folder to `Training scene`.
![training](https://user-images.githubusercontent.com/44201826/103449331-a616fc00-4c9e-11eb-8845-7e2a664fa2bf.PNG)

Then, assuming that all prerequisites are fulfilled, on the terminal it is necessary to go to the Project folder and insert one of the following commands depending if it is either the first time, or it is intended to resume the training or overwrite the training done until now.

`mlagents-learn`

`mlagents-learn --resume`

`mlagents-learn --force`

After that, all that is left is to press the play button on Unity and there will be two agents against each other.
There are some variables in the Inspector to adjust the training for the agent:
![Capture](https://user-images.githubusercontent.com/44201826/103449236-67347680-4c9d-11eb-8b48-ce5cbfa1822e.PNG)

Some Hyperparameters were twisted to try to improve the `selfplay()` process.
It is important to point out that my personal computer is not performing at its best and sometimes it would freeze Unity while the training process would be running in the background. This drawback had impact on the learning process and, so, can easily justifies some of the longer runs without significant changes (for instance, from 900k steps until 1.4M on the 1st run) or even smaller ones where it seems that the agent did nothing, which is true.

**1st Run**

`mlagents-learn UltimateChess.yaml --run-id="Hikaru_run0"`
![3](https://user-images.githubusercontent.com/44201826/103449245-86cb9f00-4c9d-11eb-8442-4a668dbdfbe7.PNG)

![1](https://user-images.githubusercontent.com/44201826/103497062-418fa480-4e38-11eb-9854-9391cef67133.PNG)
![2](https://user-images.githubusercontent.com/44201826/103497054-3fc5e100-4e38-11eb-91d3-fd288f587bbd.PNG)
![4](https://user-images.githubusercontent.com/44201826/103497058-405e7780-4e38-11eb-8e1d-1ea2caae5375.PNG)
![3](https://user-images.githubusercontent.com/44201826/103497057-405e7780-4e38-11eb-8b43-bed3780698cf.PNG)
![5](https://user-images.githubusercontent.com/44201826/103497059-40f70e00-4e38-11eb-8241-956e8b8d08a9.PNG)
![6](https://user-images.githubusercontent.com/44201826/103497060-40f70e00-4e38-11eb-95f9-ad026bee69e3.PNG)
![7](https://user-images.githubusercontent.com/44201826/103497061-40f70e00-4e38-11eb-8679-2a884830e169.PNG)
Note: The process was resumed at around 350k steps, 770k and 1.850M. This affects especially the ELO calculation

**2nd Run**

`mlagents-learn UltimateChess.yaml --run-id="Hikaru_run1"`
On this second run, some values were changed to check the impact in the long run. Ideally, the agent should learn which actions are valid or not easily, since `InvalidAction_or_do_Nothing` was `-0.01` instead of the previous `-0.0001`. Also the variable `useTables` was true during this run, which would evaluate the positioning of the pieces on the board during each valid action. Basically, the positioning of the pieces on the board would take a higher impact on the reward system.
![Capture2](https://user-images.githubusercontent.com/44201826/103498323-40f90d00-4e3c-11eb-8b14-7f71d8c8d2dd.PNG)

![10](https://user-images.githubusercontent.com/44201826/103501801-32fcb980-4e47-11eb-921c-c6704c20f23f.PNG)
![9](https://user-images.githubusercontent.com/44201826/103501803-33955000-4e47-11eb-9cc4-77672476c35f.PNG)
![8](https://user-images.githubusercontent.com/44201826/103501804-33955000-4e47-11eb-8ab3-762b59fb11f7.PNG)
![12](https://user-images.githubusercontent.com/44201826/103501805-342de680-4e47-11eb-9337-a071ed0c067d.PNG)
![11](https://user-images.githubusercontent.com/44201826/103501806-342de680-4e47-11eb-9ed7-49cd621d27da.PNG)
This approach was not beneficial due to how the evaluation was made, he would profit by standing still.

**nth Run**



<!-- ACKNOWLEDGEMENTS -->
## Acknowledgements
The bididimensional arrays are used alongside chess engines to optimize performances.
Over the project there might be situations where the Knight is mentioned as Horse and vice versa. The same happens for the Rook that can be called Tower sometimes.

**Limitations**

This game, even though functional, does not verify checks. Movements are not restricted by checking positions (which should not influence the learning process too much). 
The agent, even though can learn, if there is a situation where the action that it's trying to perform is not valid (not during training), the game will no longer advance since it has a fixed policy.
