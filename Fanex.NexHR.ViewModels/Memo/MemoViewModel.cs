namespace Fanex.NexHR.ViewModels.Memo
{
    using Fanex.NexHR.Models;
    using System;
    using System.Collections.Generic;

    public class MemoViewModel
    {
        public MemoViewModel()
        {
            MemoCategories = new List<LookupData>();
            Memos = new List<Memo>();
            Year = DateTime.Now.Year;
            Category = -1;
        }

        public int Year { get; set; }

        public int Category { get; set; }

        public IList<LookupData> MemoCategories { get; set; }

        public IList<Memo> Memos { get; set; }
    }
}
