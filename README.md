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
Then, assuming that all prerequisites are fulfilled, on the terminal it is needed to go to the Project folder and insert one of the following commands depending if it is either the first time, or it is intended to resume the training or overwrite the training done until now.

`mlagents-learn`

`mlagents-learn --resume`

`mlagents-learn --force`

After that, all is left is to press the play button on Unity.
There are some variables in the Inspector to adjust the training for the agent:
![Capture](https://user-images.githubusercontent.com/44201826/103316721-44962980-4a21-11eb-9d45-3790bd8b12bf.PNG)


<!-- USAGE EXAMPLES -->
## Rewards System
In order to allow the agent to learn sequentially, it needs to receive some rewards and adjust its actions according to the observations that it received.
Firstly, It will be explained briefly how does the learning process work.
Shortly, the agent has episodes and, in each episode it will received a certain number of observations (all the positions of the pieces) and it will try to generate an action.
Knowing that the number of observations received needs to be always the same, it was decided that the agent will receive the positions of all the pieces on the board ( it would be -1 in case of non existence) plus 8 possible queens for each player. These extra queens represents queens that can show up when a pawn reaches a tile on the opposite side of the board, summing up to **96** observations each time. (Technically, it should not appear 8 queens at any given time, but there is no way to control that. )
When it comes to generate an action, at the position `0` of the `vectorAction` it has all the possible moves that could happen.
Since also the size of `vectorAction[0]` must be always the same, this means that, for instance, for a Pawn, it must be always 4 possibilities "stored" in that size, even if it can perform one of those movements. All summed up, the result is **676** different possibilities for actions.

There are a bunch of reward values given to the agent throughout its episode:
`public float invalidAction = -0.1f;`
`public float validAction = 0.5f`
`public float wonGame = 200.0f;`
`public float lostGame = -200.0f;`
`public float doNothing = -0.1f;`

`invalidAction` is a reward given in order for the agent to learn that a particular movement is not valid. For instance, if `vectorAction[0]` has a value of 1 while a King is on *a8*, representing that the King should move one tile up and one tile to the left, is invalid and, consequentially, it should penalize the agent. On the other hand, there is a compensation if it makes a valid move: `validAction`. 
Ultimately, it receives a larger reward depending if it wins or loses:
`wonGame` and `lostGame`.
In order to incentivize the agent to eat opposite pieces, there are some rewards with that same purpose.
  `public float strengthPawn        = 1.0f;`
  `public float strengthHorse       = 3.0f;`
  `public float strengthBishop      = 3.0f;`
  `public float strengthRook        = 5.0f;`
  `public float strengthQueen       = 9.0f;`
  `public float strengthKing        = 50.0f;`
Those rewards are multiple ( `/10`) of the [relative value](https://en.wikipedia.org/wiki/Chess_piece_relative_value) of each piece.

<!-- USAGE EXAMPLES -->
## Improved Rewards System
With the goal of optimizing the learning experience, it was added some rewards systems.
The reward `incentiveToCastling` with the value of `1.0f` intends to incentivize the agent to do the castling movement which is a very powerful movement.
After a few hundred thousand simulations, the agent has already developing the knights/horses more frequently, but it would advance the king too further in the map. In order to try to control it and improve the overall positioning and valorization of the pieces, it was added some bidimensional arrays that, combined with a relative value of each piece, allows for the agent to learn more correctly. 
These bidimensional arrays basically give a value for each position of a certain piece on the board.  For instance, as referred before, if there is a king on the opposite side of the it would be really bad for that player. On the other hand, if it was on the own side of the table, it should be better. 
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

This way, it enables the agent to learn which position are more helpful or not and returns a reward. Also, it penalizes if the agent lets the opponent with a meaningful piece on a strong position.


<!-- ACKNOWLEDGEMENTS -->
## Acknowledgements

The bididimensional arrays were not designed by me at all. From what I understood it is something used alongside chess engines.
Alongside the project that might be situations where the the Knight is mentioned and Horse and vice versa. The same happens for the Rook that can be called Tower sometimes.

**Limitations**
This game, even though functional, does not verify checks. Movements are not restricted by checking positions (which should not influence the learning process too much). Also ending game conditions are not implemented.
