//
// Auto Generated Code By excel2json
// https://neil3d.gitee.io/coding/excel2json.html
// 1. 每个 Sheet 形成一个 Struct 定义, Sheet 的名称作为 Struct 的名称
// 2. 表格约定：第一行是变量名称，第二行是变量类型

// Generate From C:\Users\潘鹏\Documents\Codes\Games\TKS-R\Excels\SchemaCharacters.xlsx.xlsx

public class CharacterParam
{
	public string CharName; // 人物名字
	public string Portrait; // 头像
	public int USD; // 悟性
	public int LUCK; // 幸运
	public int INT_V; // 智力加成
	public int MOL_V; // 仁德加成
	public int CRG_V; // 勇气加成
	public int MED_V; // 医术加成
}

public class JobStartingStats
{
	public string Name; // 职业名字
	public int MHP; // 最大血量
	public int MMP; // 最大内力
	public int ATK; // 物理攻击
	public int DEF; // 物理防御
	public int MAT; // 魔法攻击
	public int MDF; // 魔法防御
	public int HIT; // 命中
	public int EVD; // 闪避
	public int SPD; // 速度
	public int MOV; // 移动量
	public int RES; // 异常状态抵抗量
	public int JMP; // 跳跃
}

public class JobGrowthStats
{
	public string Name; // 人物属性模板
	public string MHP; // 最大血量
	public string MMP; // 最大内力
	public string ATK; // 物理攻击
	public string DEF; // 物理防御
	public string MAT; // 魔法攻击
	public string MDF; // 魔法防御
	public string HIT; // 命中率
	public string EVD; // 闪避率
	public string SPD; // 速度
}


// End of Auto Generated Code
