using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTable : DataTable
{
    public List<PlayerInfo> playerDatabase = null;
    public List<TrainingInfo> trainingDatabase = null;
    public List<LevelUpCost> levelUpCostDatabase = null;
    public List<Sprite> playerSprites;
    public List<Sprite> playerFullSprites;
    public List<Sprite> playerStandingSprites;
    public List<Sprite> starsSprites = new List<Sprite>();
    public List<Sprite> berakSprites = new List<Sprite>();
    public List<Sprite> playerTypeSprites = new List<Sprite>();
    public List<Sprite> playerSkillSprites = new List<Sprite>();
    public List<SkillInfo> skillTable = new List<SkillInfo>();

    public PlayerTable() : base(DataType.Player)
    {
    }

    public override void DataAdder()
    {
        if (playerDatabase != null) { return; }

        List<Dictionary<string, string>> players = CSVReader.Read(Path.Combine("CSV", "PlayerStats"));
        playerDatabase = new List<PlayerInfo>();
        trainingDatabase = new List<TrainingInfo>();
        levelUpCostDatabase = new List<LevelUpCost>();
        playerSprites = new List<Sprite>();
        playerFullSprites = new List<Sprite>();
        playerStandingSprites = new List<Sprite>();
        playerSkillSprites = new List<Sprite>();
        this.skillTable = new List<SkillInfo>();

        foreach (var player in players)
        {
            PlayerInfo playerInfo = new PlayerInfo();
            playerInfo.code = int.Parse(player["Code"]);
            playerInfo.name = player["Name"];
            playerInfo.grade = int.Parse(player["Grade"]);
            playerInfo.type = int.Parse(player["Type"]);
            playerInfo.UniqueSkillCode = int.Parse(player["UniqueSkill"]);
            playerInfo.Potential = int.Parse(player["Type"]);
            playerInfo.info = player["Info"];
            playerInfo.cost = int.Parse(player["Cost"]);
            playerInfo.weaponType = int.Parse(player["WeaponType"]);
            playerInfo.hp.min = float.Parse(player["minHP"]);
            playerInfo.atk.min = float.Parse(player["minAtk"]);
            playerInfo.atkRate.min = float.Parse(player["minAtkRate"]);
            playerInfo.moveSpeed.min = float.Parse(player["minSpeed"]);
            playerInfo.sight.min = float.Parse(player["minSight"]);
            playerInfo.range.min = float.Parse(player["minRange"]);
            playerInfo.critical.min = float.Parse(player["minCritical"]);
            playerInfo.magazine = int.Parse(player["Mag"]);
            playerInfo.reloadingSpeed = float.Parse(player["Reload"]);
            playerInfo.accuracy.min = float.Parse(player["minAccuracy"]);
            playerInfo.reactionSpeed.min = float.Parse(player["minReaction"]);
            playerInfo.detectionRange.min = float.Parse(player["minDetection"]);
            playerInfo.atkType = int.Parse(player["AtkType"]);
            playerInfo.kitingType = int.Parse(player["KitingType"]);

            playerInfo.hp.max = float.Parse(player["maxHP"]);
            playerInfo.atk.max = float.Parse(player["maxAtk"]);
            playerInfo.atkRate.max = float.Parse(player["maxAtkRate"]);
            playerInfo.moveSpeed.max = float.Parse(player["maxSpeed"]);
            playerInfo.sight.max = float.Parse(player["maxSight"]);
            playerInfo.range.max = float.Parse(player["maxRange"]);
            playerInfo.critical.max = float.Parse(player["maxCritical"]);
            playerInfo.accuracy.max = float.Parse(player["maxAccuracy"]);
            playerInfo.reactionSpeed.max = float.Parse(player["maxReaction"]);
            playerInfo.detectionRange.max = float.Parse(player["maxDetection"]);


            playerDatabase.Add(playerInfo);
            playerSprites.Add(Resources.Load<Sprite>(
                Path.Combine("PlayerSprite", playerInfo.code.ToString())));
            playerFullSprites.Add(Resources.Load<Sprite>(
                Path.Combine("PlayerFullSprite", playerInfo.code.ToString())));
            playerStandingSprites.Add(Resources.Load<Sprite>(
                Path.Combine("PlayerStandingSprite", playerInfo.code.ToString())));
            playerSkillSprites.Add(Resources.Load<Sprite>(
                Path.Combine("Skill Image", playerInfo.code.ToString())));
        }

        List<Dictionary<string, string>> training = CSVReader.Read(Path.Combine("CSV", "TrainingStatTable"));
        foreach (var item in training)
        {
            TrainingInfo newTrain = new TrainingInfo();
            newTrain.id = int.Parse(item["ID"]);
            newTrain.type = item["Type"] switch
            {
                "�̵��ӵ�" => TrainingType.MoveSpeed,
                "�þ߰Ÿ�" => TrainingType.Sight,
                "�����Ÿ�" => TrainingType.Range,
                "��������" => TrainingType.DetectionRange,
                "���߷�" => TrainingType.Accuracy,
                "�����ӵ�" => TrainingType.ReactionSpeed,
                "���ݼӵ�" => TrainingType.AtkRate,
                "ũ��Ƽ�� Ȯ��" => TrainingType.Critical,
                "���ݷ�" => TrainingType.Atk,
                "�ִ� ü��" => TrainingType.Hp,
                _ => TrainingType.Hp
            };
            newTrain.level = int.Parse(item["Level"]);
            newTrain.value = float.Parse(item["Value"]);
            newTrain.needPotential = int.Parse(item["Potential"]);
            trainingDatabase.Add(newTrain);
        }

        List<Dictionary<string, string>> levelCost = CSVReader.Read(Path.Combine("CSV", "UpgradeCostTable"));
        foreach (var item in levelCost)
        {
            LevelUpCost newTrain = new LevelUpCost();
            newTrain.level = int.Parse(item["Level"]);
            newTrain.xp = int.Parse(item["Xp"]);
            newTrain.cost = int.Parse(item["Cost"]);

            levelUpCostDatabase.Add(newTrain);
        }

        for (int i = 0; i < 3; i++)
        {
            starsSprites.Add(Resources.Load<Sprite>(Path.Combine("Stars", i.ToString())));
        }
        for (int i = 0; i < 3; i++)
        {
            berakSprites.Add(Resources.Load<Sprite>(Path.Combine("Break", i.ToString())));
        }
        for (int i = 0; i < 4; i++)
        {
            playerTypeSprites.Add(Resources.Load<Sprite>(Path.Combine("PlayerType", (i + 1).ToString())));
        }

        List<Dictionary<string, string>> skillTable = CSVReader.Read(Path.Combine("CSV", "skilltable"));
        foreach (var item in skillTable)
        {
            SkillInfo newSkill = new SkillInfo();
            newSkill.ID = int.Parse(item["skillID"]);
            newSkill.name = int.Parse(item["skillName"]);
            newSkill.toolTip = int.Parse(item["skillTooltip"]);
            newSkill.icon = Resources.Load<Sprite>(Path.Combine("SkillIcon", newSkill.ID.ToString()));
            this.skillTable.Add(newSkill);
        }
    }

    public int PlayerIndexSearch(int code)
    {
        return playerDatabase.FindIndex(player => player.code == code);
    }

    public PlayerInfo GetPlayerInfo(int code)
    {

        return playerDatabase.Find(player => player.code == code);
    }

    public Sprite GetPlayerSprite(int code)
    {
        return playerSprites[PlayerIndexSearch(code)];
    }
    public Sprite GetPlayerFullSprite(int code)
    {
        return playerFullSprites[PlayerIndexSearch(code)];
    }
    public Sprite GetPlayerStandingSprite(int code)
    {
        return playerStandingSprites[PlayerIndexSearch(code)];
    }

    public float CalculateCurrStats(GrowableStats stats, int level)
    {
        return stats.min + (stats.gap * level);
    }

    public TrainingInfo GetTrainingInfo(int id)
    {
        return trainingDatabase.Find(train => train.id == id);
    }

    public int GetLevelUpXp(int level)
    {
        return levelUpCostDatabase.Find(a => a.level == level).xp;
    }

    public int GetLevelUpCost(int level)
    {
        return levelUpCostDatabase.Find(a => a.level == level).cost;
    }

    public SkillInfo GetSkillInfo(int code)
    {
        return skillTable.Find(a => a.ID == code);
    }

    public Sprite GetPlayerSkillSprite(int code)
    {
        return playerSkillSprites[PlayerIndexSearch(code)];
    }
}
