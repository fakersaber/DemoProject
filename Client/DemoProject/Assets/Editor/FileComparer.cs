using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileComparer : IComparer<FileInfo>
{
    public int Compare(FileInfo a, FileInfo b)
    {
        int index_a_dot = a.Name.LastIndexOf('.');
        int index_a__ = a.Name.LastIndexOf('_');
        int num_a = Convert.ToInt32(a.Name.Substring(index_a__ + 1, index_a_dot - index_a__ - 1));

        int index_b_dot = b.Name.LastIndexOf('.');
        int index_b__ = b.Name.LastIndexOf('_');
        int num_b = Convert.ToInt32(b.Name.Substring(index_b__ + 1, index_b_dot - index_b__ - 1));

        return num_a < num_b ? -1 : 1;
    }
}
