/*
 * The following section belongs or is derived from LiveSplit.Subsplits under the MIT License of parent project LiveSplit
 * Copyright (c) 2013 Christopher Serr and Sergey Papushin
 * Licensed under the MIT License
 * Original source: https://github.com/LiveSplit/LiveSplit.Subsplits
 */

using LiveSplit.Model;
using LiveSplit.TimeFormatters;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public class SectionList
{
    public class Section
    {
        public readonly int startIndex;
        public readonly int endIndex;

        public Section(int startIndex, int endIndex)
        {
            this.startIndex = startIndex;
            this.endIndex = endIndex;
        }

        public bool SplitInRange(int splitIndex)
        {
            return splitIndex >= startIndex && splitIndex <= endIndex;
        }

        public int GetSubsplitCount()
        {
            return endIndex - startIndex;
        }
    }

    public int count;
    public List<int> anchorIndices;

    public List<Section> Sections;

    public void UpdateSplits(IRun splits)
    {
        count = splits.Count();
        if (count == 0) return;

        Sections = new List<Section>();
        anchorIndices = new List<int>();

        for (int i = 0; i < count; i++)
        {
            if (!splits[i].Name.StartsWith("- ")) anchorIndices.Add(i);
        }

        if (anchorIndices.Count == 0)
        {
            Sections.Add(new Section(0, count - 1));
            return;
        }

        Debug.Print(anchorIndices.Count() + " section breakpoints found among " + count + " splits on positions " + anchorIndices);

        Sections.Add(new Section(0, anchorIndices[0]));

        for(int i = 0; i < anchorIndices.Count - 1; i++)
        {
            Sections.Add(new Section(anchorIndices[i] + 1, anchorIndices[i + 1]));
        }

        if(anchorIndices.Last() < count - 1)
        {
            Sections.Add(new Section(anchorIndices.Last() + 1, count - 1));
        }
        //for(int splitIndex = splits.Count() - 1; splitIndex >= 0; splitIndex--)
        //{
        //    int sectionIndex = splitIndex;
        //    while((splitIndex > 0) && splits[splitIndex - 1].Name.StartsWith("- "))
        //    {
        //        splitIndex--;
        //    }

        //    Sections.Insert(0, new Section(splitIndex, sectionIndex));
        //}
    }

    public int GetSection(int splitIndex)
    {
        for(int i = 0; i < Sections.Count; i++)
        {
            if (splitIndex >= Sections[i].startIndex && splitIndex <= Sections[i].endIndex) return i;
        }

        return -1;
        //foreach (Section section in Sections)
        //{
        //    if (section.SplitInRange(splitIndex))
        //    {
        //        return Sections.IndexOf(section);
        //    }
        //}

        //return -1;
    }

    public bool IsMajorSplit(int splitIndex)
    {
        int sectionIndex = GetSection(splitIndex);
        return sectionIndex != -1 && splitIndex == Sections[sectionIndex].endIndex;
        //    if (sectionIndex == -1)
        //    {
        //        return true;
        //    }

        //    return splitIndex == Sections[sectionIndex].endIndex;
    }

    public int GetMajorSplit(int splitIndex)
    {
        int sectionIndex = GetSection(splitIndex);
        if (sectionIndex == -1)
        {
            return splitIndex;
        }
        return Sections[sectionIndex].endIndex;
    }

    public int GetNextMajorSplit(IRun splits, int splitIndex)
    {
        for (int i = splitIndex + 1; i < splits.Count(); i++)
        {
            if (IsMajorSplit(i)) return i;
        }
        return splits.Count();
    }
}