# Console Maze Multiplayer
## Game Overview
In this game, a group of players will explore their own independent maze,
gather info, and communicate with one another via messages through a 
specific terminal in each player's maze.

Important items and information are scattered between all players. They 
must communicate and solve puzzles independently and together to escape.

## How to Escape
Scattered throughout the mazes are clues and items that players need to
find. The most important of which are two kinds of floppy discs. One kind
contains software that can be loaded onto that player's terminal. The other
kind contains "man page" files for specific software. Man pages will never
be found in the same maze that their corresponding software is found in.

By reading the man pages, players will learn how to use the software, and
must clearly communicate those instructions to the player who has the
software for it, as well as gather any relevant info needed for the
software from all players. Once the player with the software feels
confident in their understanding of how to use it, and what command args
need to be used for the specific maze they're in, they can attempt to
use the software. Using it successfully will contribute towards escaping
the maze. Any mistake in the command and the team will receive a strike.
If they receive 3 strikes, the game is over and they lose.

### Example
bscit finds software called "mke". Annie finds a man page for software
called "Mad Key Effigy". They mention what they've found to each other
and recognize the man page belongs to the software due to the acronym.
Annie looks at the man page:

```
Mad Key Effigy
Usage: mke [-l] [-f] [-s]

Options:
  -l --list     List all options.
  -f --flash    Flash a specific key.
  -s --simple   Execute the "simple" version of the command.

About:
Used to flash keys from storage. 
Refer to the table below to determine which key(s) to use. 
-------------------------------------
1. If there are less than 5 keys in the list, use the
    "simple" command.
2. If at least two keys in the list begin with "f", use the
    second one of the two.
3. If at least three keys all begin with the same letter, use
    the last key in the list.
4. If the sum of the vowels in the list is greater than 20,
    use the first key in the list that contains a vowel.
5. If none of the above, use the fourth key in the list if it
    has an even number of letters, otherwise use the 5th key.
-------------------------------------
```

Annie tells bscit to run the command "mke -l" and reply with how many
items are returned. bscit runs the command, and sees a list of words: 
``frog, hush, sock, electric, seven, burger, elf, mississippi``. 
bscit tells Annie there are eight words. Annie asks bscit if at least
two begin with the letter "f". bscit says no. Annie asks if at least
three keys all begin with the same letter. bscit says no. Annie asks 
if the sum of all vowels is greater than 20. bscit says no, there 
are 15. Annie asks what the fourth key is. bscit says "electric".
Annie says to run the command `mke -f electric`. bscit runs the
command and the whole team is notified that one puzzle has been
solved.
