using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PckStudio.Classes.FileTypes
{
    public class PCKAudioFile
    {
		public class InvalidCategoryException : Exception
        {
			public InvalidCategoryException(string message) : base(message)
			{ }
		}

		public readonly int type = 1;

		public AudioCategory[] Categories { get; } = new AudioCategory[8];

		public Dictionary<string, string> Credits { get; } = new Dictionary<string, string>();

		public class AudioCategory
        {
			public enum EAudioType : int
			{
				Overworld,
				Nether,
				End,
				Creative,
				Menu,
				Battle,
				Tumble,
				Glide,
				Unused,
			}

			public enum EAudioParameterType : int
			{
				unk0, // dimension music
				unk1, // unused music ?
			}

			public string Name { get; set; } = string.Empty;
            public EAudioParameterType parameterType { get; }
			public EAudioType audioType { get; }
			public List<string> SongNames { get;  } = new List<string>();

			public AudioCategory(EAudioParameterType parameterType, EAudioType audioType)
			{
				this.parameterType = parameterType;
				this.audioType = audioType;
			}

			public AudioCategory(string name, EAudioParameterType parameterType, EAudioType audioType) : this(parameterType, audioType)
            {
				Name = name;
            }
        }

		public string[] GetCredits() => Credits.Values.ToArray();

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
			foreach (var credit in Credits)
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
			foreach (var credit in credits)
            {
				AddCredit(credit);
            }
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
			Credits.Add($"IDS_CREDIT{(Credits.Count > 1 ? $"_{Credits.Count}" : string.Empty)}", credit);
		}

		public void AddCreditId(string creditId) => Credits.Add(creditId, string.Empty);


		/// <exception cref="InvalidCategoryException"></exception>
		public bool HasCategory(AudioCategory.EAudioType category) => GetCategory(category) is AudioCategory;

		/// <exception cref="InvalidCategoryException"></exception>
		public AudioCategory GetCategory(AudioCategory.EAudioType category)
		{
			if (category < AudioCategory.EAudioType.Overworld ||
				category > AudioCategory.EAudioType.Unused)
				throw new InvalidCategoryException(nameof(category));
			return Categories[(int)category];
		}

		/// <exception cref="InvalidCategoryException"></exception>
		public bool TryGetCategory(AudioCategory.EAudioType category, out AudioCategory audioCategory)
        {
			if (GetCategory(category) is AudioCategory a)
            {
				audioCategory = a;
				return true;
            }
			audioCategory = null;
			return false;
        }
		
		public int GetCategoryCount()
		{
			int count = 0;
			Array.ForEach(Categories, c => { if (c is not null) count++; });
			return count;
		}

		/// <returns>True when category was created, otherwise false</returns>
		/// <exception cref="InvalidCategoryException"></exception>
		public bool AddCategory(AudioCategory.EAudioParameterType parameterType, AudioCategory.EAudioType category)
		{
			if (category < AudioCategory.EAudioType.Overworld ||
				category > AudioCategory.EAudioType.Unused)
				throw new InvalidCategoryException(nameof(category));
			bool exists = HasCategory(category);
			if (!exists) Categories[(int)category] = new AudioCategory(parameterType, category);
			return !exists;
		}

		/// <returns>True when category was created, otherwise false</returns>
		/// <exception cref="InvalidCategoryException"></exception>
		public bool AddCategory(AudioCategory.EAudioType category)
			=> AddCategory(AudioCategory.EAudioParameterType.unk0, category);

		/// <returns>True when category was removed, otherwise false</returns>
		/// <exception cref="InvalidCategoryException"></exception>
		public bool RemoveCategory(AudioCategory.EAudioType category)
        {
			bool exists = HasCategory(category);
			if (exists) Categories[(int)category] = null;
			return exists;
		}

	}
}
