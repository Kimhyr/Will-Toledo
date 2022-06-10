using System;
using System.Collections.Generic;
using Discord;

namespace PenileNET.Utilities {
    public class GeneralTools {
        public static Color StringToColor(string color) {
            var newColor = new List<int>(3);

            var colorArr = color.Split(' ');
            if (colorArr.Length > 3) {
                throw new Exception();
            }

            foreach (var i in colorArr) {
                if (int.TryParse(i, out var ret)) {
                    newColor.Add(ret);
                } else {
                    throw new Exception();
                }
            }

            return new Color(newColor[0], newColor[1], newColor[2]);
        }

        public static List<T> GetSorted<T>(List<T> list) {
            list.Sort();
            list.Reverse();

            return list;
        }
    }
}