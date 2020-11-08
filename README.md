# Gomoku
2 human or vs 1 computer five in a row game using minimax algorithm with alpha beta pruning

## Status

- Maximum depths greater than 1 are currently not behaving as expected (computer simple mistakes)

## Game Description

Gomoku, also known as Five in a Row, is a two-player board game. Black goes first, then players
alternate turns until the outcome is decided. To win, a player must get
five in a row vertically, horizontally, or diagonally; if getting five
in a row becomes impossible for either player then the match is a tie.

## Algorithm Description

Minimax algorithm is used to traverse an adversarial game tree and get the best move 
for the current player.  Alpha beta pruning is used to avoid unnecessary calculations 
at minimal loss of accuracy.  Bounded lookahead is used to limit the search depth for 
significant performance gains, but at a major cost of accuracy.  Breadth is limited by
maintaining a list of possible moves that are constrained to tiles adjacent to existing
moves; can maintain accuracy with proper implementation.

## Static Evaluation

The static evaluation function calculates the score of the current board state for a player.
Some factors that are considered for evaluation in a typical game of Gomoku:
- Win (five in a row)
- Variable threats: 
  - Four in a row
  - Three in a row
  - Score affected by:
    - Number of open ends
    - Number of available tiles
    - Gaps
- Low/Zero threat:
  - Less than five consecutive pieces and: 
    - No open ends or 
    - Available tiles on both ends less than five
  - Less than four consecutive pieces and only one open end
