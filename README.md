# YGOLOTDPatchDraft
Patching Tool for Yu-Gi-Oh: Legacy of the Duelist (LOTD) to automatically change the Battle Pack 1 draft pool and AI decks.

# Link Evolution
The Releases section also provides a built binary for LOTD: Link Evolution (2020). The code for this can be found in the `link_evolution_2020` branch.

# Features
Supports .ydk (YGOPRO) or .ydc (directly from LOTD) deck files for the enemy AI decks, and .ydk (YGOPRO) or .json (Booster) files for the packs from which the draft/sealed cards are drawn.

Just put .ydk decks you want the AI to use inside DECK_DATABASE and .ydk lists or JSON boosters you want to draft from inside PUT_DRAFT_DECKS_PACKS_HERE

# Thanks/Credits
Thanks to [Arefu](https://github.com/Arefu/) for the [Wolf](https://github.com/Arefu/Wolf) Project, which helped a lot in reducing
some of the required busywork for this tool. (Especially extracting/packing files from LOTD.
The adapted files from [Wolf](https://github.com/Arefu/Wolf) can be found inside the "Wolf" directory. I fixed some stuff in Cyclone for
packing decks.zib/packs.zib and adapted them for this all-in-one tool.

# Usage
Upon program startup, you will be prompted for your LOTD installation path. After choosing that, you'll see this screen:

![Screen](screen.png?raw=true "Screen")

This is where you're able to access all the functions. Just in case - backup your original YGO_DATA.dat and YGO_DATA.toc files in your
game installation, just in case something goes wrong.

# Extract YGO:LOTD Game Files
This creates a YGO_DATA working directory in the same directory as the .exe. (This needs ~1.1GB space, be prepared for that.)
This working directory is used by the program to patch and reassemble the game files.

# Unpack Decks/Packs
This unpacks the decks.zib/packs.zib files in the extracted working directory to the directories decks.zib_UNPACKED/packs.zib_UNPACKED respectively.
These folders contain the raw unpacked game files for the packs and decks which the tool is manipulating.

# Extract Draft Deck from LOTD Save
This prompts you for your LOTD savegame.dat and allows you to extract your current Battle Pack 1 Draft Deck into the DECK_DATABASE, to use for AI Deck randomization.

# Patch bpack_BattlePack
This takes all the selected .ydc/.ydk files in the "Packs" list, randomizes the cards and patches the games' Battle Pack 1 draft cards.

# Patch AI Draft Decks/Packs
This takes all the selected .ydc/.ydk files in the "Decks" list, and randomly picks 20 decks to patch the Battle Pack 1 AI enemy decks.

# Pack YGO_DATA files
This packs all patched/modified files from the YGO_DATA working directory to YGO_DATA.dat and YGO_DATA.toc files.

# Copy Patched Files to Game Directory
This copies the final packed files to the LOTD game directory. This *will* overwrite your files, so make backups if necessary.

# Patch, Pack, Copy all
Automatically patches the draft packs and enemy AI decks for Battle Pack 1, packs all the files and copies them to the LOTD game directory. 
This is basically an all-in-one button for the stuff above.

# Simulate Rarity (only .json) (CheckBox)
This takes the rarity data in the Booster Pack JSON files into account and simulates simple rarities for the Battle Pack 1 draft mode.

# Decks-List:
Checking items in this list allows them to be used for the AI Decks in Battle Pack 1 Draft mode. (And also Sealed Mode)

## Show only Checked (CheckBox)
Only shows checked decks in the list.

## All (CheckBox)
Checks/Unchecks all decks.

## Filter (TextBox)
Allows you to filter/search the DECK_DATABASE directory

# Packs-List:
Checking items in this list allows them to be used for the cards drawn in Battle Pack 1 Draft mode. (And also Sealed Mode)

## Show only Checked (CheckBox)
Only shows checked packs in the list.

## All (CheckBox)
Checks/Unchecks all packs.

## Filter (TextBox)
Allows you to filter/search the PUT_DRAFT_DECKS_PACKS_HERE directory (all packs)
