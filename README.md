
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
    <li><a href="#usage">Usage </a></li>
    <li><a href="#training-the-agent">Training The Agent</a></li>
    <li><a href="#rewards-system">Rewards System</a></li>
    <li><a href="#improved-rewards-system">Improved Reward System</a></li>
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
For preferencial use, an environment was created using Anaconda for training purposes and later on tensorboard for tracking the learning process.

`tensorboard --logdir=results --port 6006`

<!-- USAGE EXAMPLES -->
## Usage
The Project has a simple menu that allows the user to choose to either play against another user on the same computer. The game proceeds by simply clicking on a tile and select where to move that piece. Some highlights are shown over the tiles to help the user know where that same piece can, in fact, move.

![simple menu](https://user-images.githubusercontent.com/44201826/103315976-07309c80-4a1f-11eb-99cf-dfe677c6378e.PNG)

![UltimateCHess2](https://user-images.githubusercontent.com/44201826/102122430-5e3e4b00-3e3d-11eb-9814-3c8ebdeb32f3.PNG)


<!-- USAGE EXAMPLES -->
## Training the Agent

In order to train the agent, it is necessary to change the scene in the Scene folder to `Training scene`.
Then, assuming that all prerequisites are fulfilled, on the terminal it is necessary to go to the Project folder and insert one of the following commands depending if it is either the first time, or it is intended to resume the training or overwrite the training done until now.

`mlagents-learn`

`mlagents-learn --resume`

`mlagents-learn --force`

After that, all that is left is to press the play button on Unity and there will be two agents against each other.
There are some variables in the Inspector to adjust the training for the agent:
![Capture](https://user-images.githubusercontent.com/44201826/103394150-a9837980-4b1e-11eb-8964-98c21bba785d.PNG)
Those agents will have different teams, enabling `selfplay()`. This means that each agent will be playing against a snapshot of itself (with a fixed policy). Knowing this, one way to know if the agent learn is through the [ELO](https://en.wikipedia.org/wiki/Elo_rating_system), which will represent the skill level of the agent. This skill rate is updated in case off victory or loss when facing the older snapshot.
Some Hyperparameters were twisted to try to improve the `selfplay()` process.
![2](https://user-images.githubusercontent.com/44201826/103394209-f8c9aa00-4b1e-11eb-831d-6754f4e127c3.PNG)

`mlagents-learn --resume UltimateChess.yaml --run-id="HikaruAsASister"`

<!-- USAGE EXAMPLES -->
## Rewards System
In order to allow the agent to learn sequentially, it needs to receive some rewards and adjust its actions according to the observations made.
Firstly, it will be explained briefly how the learning process works.
Shortly, the agent has episodes and, in each episode it will perform a certain number of observations (all the positions of the pieces) and it will try to generate an action.
Knowing that the number of observations made needs to be always the same, it was decided that the agent will receive the positions of all the pieces on the board ( it would be -1 in case of non existence) plus 8 possible queens for each player. These extra queens represent queens that can show up when a pawn reaches a tile on the opposite side of the board, summing up to **96** observations each time. (Technically, it should not appear 8 queens at any given time, but there is no way to control that. )
When it comes to generating an action, at the position `0` of the `vectorAction` contains one of all the possible moves that could appear.
Also, since the size of `vectorAction[0]` must be always the same, this means that, for instance, for a Pawn, there must be always 4 possibilities "stored" in that `vectorAction` position (even if it can only perform one of those movements). All summed up, the result is **676** different possibilities for actions.

There are a bunch of reward values given to the agent throughout its episode:
`public float invalidAction_or_doNothing= -0.05f;`

`public float validAction = 0.05f`

`public float wonGame = 1.0f;`

`public float lostGame = -1.0f;`


`invalidAction` is a reward given in order for the agent to learn that a particular movement is not valid. For instance, if `vectorAction[0]` has a value of 1 while a King is on *a8*, representing that the King should move one tile up and one tile to the left, it is invalid and, consequentially, should penalize the agent. On the other hand, there is a compensation by making a valid move: `validAction`. 
Ultimately, it receives a larger reward depending if it wins or loses:
`wonGame` and `lostGame`.


`public float strengthPawn        = 0.001f;`
`public float strengthHorse       = 0.003f;`
`public float strengthBishop      = 0.003f;`
`public float strengthRook        = 0.005f;`
`public float strengthQueen       = 0.009f;`
`public float strengthKing        = 0.05f;`

Those strengths were set according to a [relative value](https://en.wikipedia.org/wiki/Chess_piece_relative_value) of each piece. The rewards of eating a piece of certain strength is divided by 10, in order to incentivize the agent to eat opposite pieces.


## Improved Rewards System
With the goal of optimizing the learning experience, some extra rewards systems were added.
The reward `incentiveToCastling` with the value of `1.0f` intends to incentivize the agent to perform the castling movement which is a very powerful movement. On the same note, since converting a Pawn into a Queen is really good, `incentiveToConverting` was added.
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

This way, it enables the agent to learn which positions are more helpful or not and returns a reward. Also, it penalizes if the agent lets the opponent have a meaningful piece on a strong position.
Other aspect that was twisted was the fact that the relative strength of each piece on the table is only verified if the agent chooses a valid action. This is important once the agent started to realized it could continuously choose invalid actions and still profit from them.

<!-- ACKNOWLEDGEMENTS -->
## Acknowledgements

The bididimensional arrays were not designed by me at all. It is something used alongside chess engines to optimize performances.
Over the project there might be situations where the Knight is mentioned as Horse and vice versa. The same happens for the Rook that can be called Tower sometimes.

**Limitations**
This game, even though functional, does not verify checks. Movements are not restricted by checking positions (which should not influence the learning process too much). Also ending game conditions are not implemented and pawns can only spawn queens.
