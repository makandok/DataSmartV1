using System;

namespace DataSmart.projects.ppx
{
    public class PPDeviceSizes
    {
        public PPDeviceSizes(DateTime placementDate)
        {
            var dateValue = string.Format("{0}/{1}/{2}",
                placementDate.Day < 10 ? "0" + placementDate.Day : placementDate.Day.ToString(),
                placementDate.Month < 10 ? "0" + placementDate.Month : placementDate.Month.ToString()
                , placementDate.Year);
            ddMMyyy = dateValue;
            PlacementDate = placementDate;
            Unk = A = B = C = D = E = 0;
        }
        public DateTime PlacementDate { get; set; }
        public string ddMMyyy { get; set; }
        public int A { get; set; }
        public int B { get; set; }
        public int C { get; set; }
        public int D { get; set; }
        public int E { get; set; }
        public int Unk { get; set; }

        public static string getHeader()
        {
            return "Date,           \t\tA,\tB,\tC,\tD,\tE,\tUnk,\t\t\tTotal";
        }

        public string toDisplay()
        {
            return string.Format("{0},\t\t{1},\t{2},\t{3},\t{4},\t{5},\t{6},\t\t\t\t{7}", ddMMyyy, A, B, C, D, E, Unk, (A + B + C + D + E + Unk));
        }

        internal void Add(string deviceSize)
        {
            if (string.IsNullOrWhiteSpace(deviceSize))
            {
                Unk += 1;
                return;
            }

            var asLower = deviceSize.ToLowerInvariant();
            if (asLower == "a")
                A += 1;
            else if (asLower == "b")
                B += 1;
            else if (asLower == "c")
                C += 1;
            else if (asLower == "d")
                D += 1;
            else if (asLower == "e")
                E += 1;
            else
                Unk += 1;
        }
    }
}