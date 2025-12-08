using System;
using System.Collections.Generic;
using System.Linq;
using OMI.Formats.Languages;

namespace PckStudio.Core.FileFormats
{
    public sealed class PckAudioFile
    {
		public sealed class InvalidCategoryException(string message) : Exception(message) { }

        public readonly int Type = 1;

        public AudioCategory[] Categories => Array.FindAll(_categories, c => c is not null);
        private AudioCategory[] _categories { get; } = new AudioCategory[9];

		public Dictionary<string, string> Credits { get; } = new Dictionary<string, string>();
		public enum Category : int
		{
			Overworld,
			Nether,
			End,
			Creative,
			Menu,
			Battle,
			Tumble,
			Glide,
			BuildOff,
		}

		public enum EAudioParameterType : int
		{
			Unk0, // dimension music
			Unk1, // unused music ?
		}

		public sealed class AudioCategory
        {
			public string Name { get; set; } = string.Empty;
			public Category AudioType { get; }
			public List<string> SongNames { get;  } = new List<string>();
            public EAudioParameterType ParameterType { get; }

			public AudioCategory(string name, EAudioParameterType parameterType, Category audioType)
			{
				Name = name;
				ParameterType = parameterType;
				AudioType = audioType;
			}
		}

		public string[] GetCredits() => Credits.Values.ToArray();
		public string GetCreditsString() => string.Join("\n", Credits.Values.ToArray());

		public void AddCredits(params string[] credits)
        {
			foreach (var credit in credits)
            {
				AddCredit(credit);
			}
        }

		/// <summary>
		/// Applies internal Credits to loc file
		/// </summary>
		public void ApplyCredits(LOCFile locFile)
        {
            foreach (KeyValuePair<string, string> credit in Credits)
            {
				locFile.SetLocEntry(credit.Key, credit.Value);
            }
        }

		/// <summary>
		/// Clears and sets the new supplied <paramref name="credits"/>
		/// </summary>
		public void SetCredits(params string[] credits)
        {
			Credits.Clear();
			AddCredits(credits);
        }

		public bool SetCredit(string creditId, string s)
        {
			if (!Credits.ContainsKey(creditId))
				return false;
			Credits[creditId] = s;
			return true;
		}

		public void AddCredit(string credit)
		{
			Credits.Add($"IDS_CREDIT{(Credits.Count > 0 ? $"_{Credits.Count+1}" : string.Empty)}", credit);
		}

		public void AddCreditId(string creditId) => Credits.Add(creditId, string.Empty);


		/// <exception cref="InvalidCategoryException"></exception>
		public bool HasCategory(Category category) => GetCategory(category) is AudioCategory;

		/// <exception cref="InvalidCategoryException"></exception>
		public AudioCategory GetCategory(Category category)
		{
			if (!Enum.IsDefined(typeof(Category), category))
				throw new InvalidCategoryException(nameof(category));
			return _categories[(int)category];
		}

		public bool TryGetCategory(Category category, out AudioCategory audioCategory)
        {
			if (GetCategory(category) is AudioCategory audioCat)
            {
				audioCategory = audioCat;
				return true;
            }
			audioCategory = null;
			return false;
        }

		/// <returns>True when category was created, otherwise false</returns>
		/// <exception cref="InvalidCategoryException"></exception>
		public bool AddCategory(string name, Category category, EAudioParameterType parameterType)
        {
			if (!Enum.IsDefined(typeof(Category), category))
				throw new InvalidCategoryException(nameof(category));
			bool exists = HasCategory(category);
			if (!exists)
				_categories[(int)category] = new AudioCategory(name, parameterType, category);
			return !exists;
		}

		/// <returns>True when category was created, otherwise false</returns>
		/// <exception cref="InvalidCategoryException"></exception>
		public bool AddCategory(Category category) => AddCategory("", category, EAudioParameterType.Unk0);

		/// <returns>True when category was removed, otherwise false</returns>
		/// <exception cref="InvalidCategoryException"></exception>
		public bool RemoveCategory(Category category)
        {
			bool exists = HasCategory(category);
			if (exists)
				_categories[(int)category] = null;
			return exists;
		}

	}
}
