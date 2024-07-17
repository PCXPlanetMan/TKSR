using System;
using System.Collections.Generic;

namespace TKSR
{
	[Serializable]
	public class SchemaItems
	{
		public List<BaseItems> BaseItems;
		public List<Medics> Medics;
		public List<Props> Props;
		public List<Weapons> Weapons;
		public List<Armors> Armors;
		public List<Accessories> Accessories;
		public List<Specials> Specials;
	}

	public class ItemMedicConfig : BaseItems
	{
		public Medics MedicData;
	}

	public class ItemPropConfig : BaseItems
	{
		public Props PropData;
	}

	public class ItemWeaponConfig : BaseItems
	{
		public Weapons WeaponData;
	}

	public class ItemArmorConfig : BaseItems
	{
		public Armors ArmorData;
	}

	public class ItemAccessoryConfig : BaseItems
	{
		public Accessories AccessoryData;
	}

	public class ItemSpecialConfig : BaseItems
	{
		public Specials SpecialData;
	}
}