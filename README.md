LiveSplit.ChapterSumOfBest
===========================

This LiveSplit component calculates and displays a sum of best times for subsplit groups, based on the best segments within each chapter or group of splits to help runners compare current progress with the best possible time for a game chapter or chained set of splits.

### Install
-------

Download the latest version from the project releases page and place it in the `Components` folder of your LiveSplit installation. After installation, the component appears under the **Information** category.

### Usage
-----

Add the component to your layout, select the value accuracy and timing mode from the settings or use the default options. The component will compute a sum of the best segments of the current subsplit group segments, giving you a running total for the best possible completion of each chapter.

### Timing Mode
-----------

The component supports both timing modes:

* Real Time - compares the current run using LiveSplit's standard Real Time clock.
* Game Time - compares the current run using LiveSplit's in-game timing clock.

### Behavior
--------

The component evaluates each subsplit's section (as defined per the program's code) and sum the best recorded times for that section's subsplits. The result is a per section value that allow to compare more easily chapters of a game of sets of splits that work closely together and can have a chain effect

If a group has not recorded enough data, the component will indicate the value "". This is normal when the run history is in its first run or the data has been deleted.

### Notes
-----

* Use the component with split files that define chapter or subsplit group boundaries clearly.
* The component is designed to work with data from completed segments and chapter groups.
* It is intended for runners who want a best-of grouping view rather than a standard split-by-split comparison.

### Issues and suggestions
-----

Any bug or issue found can be posted on the issues page or sent to my personal e-mail, suggestions are also appreciated.

### Thanks
-----

 * (JeepyGMI)[https://www.youtube.com/@JeepyGmi] for the idea of this addon.
 * TheSoundDefense for the repository (ResetChance)[https://github.com/TheSoundDefense/LiveSplit.ResetChance] as the base for learning how to code for LiveSplit and not having to vibe code this repo from the very beggining.
 * (LiveSplit)[https://github.com/LiveSplit] and it's team in general for making such a great tool for sppedrunners.