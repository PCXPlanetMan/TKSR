//
// Auto Generated Code By excel2json
// https://neil3d.gitee.io/coding/excel2json.html
// 1. 每个 Sheet 形成一个 Struct 定义, Sheet 的名称作为 Struct 的名称
// 2. 表格约定：第一行是变量名称，第二行是变量类型

// Generate From C:\Users\潘鹏\Documents\Codes\Games\TKS-R\Excels\SchemaItems.xlsx.xlsx

public class BaseItems
{
	public int Id; // 物品代码
	public int Authority; // 使用权限
	public string I2Name; // 物品I2名字
	public string Icon; // 缩略图
	public int Price; // 价格
	public int Type; // 物品类型
	public string I2DVariable; // Dialogue变量
}

public class Medics
{
	public int Id; // 唯一标识
	public int ReHP; // 恢复生命
	public int ReMP; // 恢复内力
	public string PlusBuff; // 施加Buff
	public string MinusBuff; // 消除Buff
}

public class Props
{
	public int Id; // 唯一标识
	public int AddMHP; // 增加生命上限
	public int AddMMP; // 增加内力上限
	public int AddATK; // 增加攻击
	public int AddDEF; // 增加防御
	public int AddHIT; // 增加命中
	public int AddEVD; // 增加闪避
	public int AddSPD; // 增加速度
	public int AddLUK; // 增加幸运
	public int AddUSD; // 增加悟性
	public int AddMED; // 增加医术点数
	public int AddSKP; // 增加技能点数
}

public class Weapons
{
	public int Id; // 唯一标识
	public int AddATK; // 增加攻击
	public int AddHIT; // 增加命中
	public string PlusBuff; // 施加Buff
	public string Extra; // 附加特效
}

public class Armors
{
	public int Id; // 唯一标识
	public int AddDEF; // 增加防御
	public int AddEVD; // 增加闪避
	public string Extra; // 附加特效
}

public class Accessories
{
	public int Id; // 唯一标识
	public int AddATK; // 增加防御
	public int AddDEF; // 增加防御
	public int AddHIT; // 增加命中
	public int AddEVD; // 增加闪避
	public int AddSPD; // 增加速度
	public int AddLUK; // 增加幸运
	public int AddUSD; // 增加移动
	public int AddMOV; // 增加移动
	public string Extra; // 附加特效
}

public class Specials
{
	public int Id; // 唯一标识
}


// End of Auto Generated Code
