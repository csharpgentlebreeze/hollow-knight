using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    [Serializable]
    public class Parameter
    {
        public float moveSpeed = 5f;

        public float maxComboTime = 1f;
        public float slashIntervalTime = 0.2f;
        public int slashDamage = 1;
        
        public float hurtForce = 5f;
        
        public float dashIntervalTime = 0.7f;
        
        public float downRecoilForce = 5f;
        public float recoilForce = 5f;
        
        public float jumpForce = 9f;
        
        public float dashSpeed = 15f;
    }
}

namespace Enemy
{
    [Serializable]
    public class Parameter
    {
        public virtual int health { get; set;}
        public virtual int damage { get; set;}
        public virtual float moveSpeed { get; set;}
        
        public virtual float detectDistance { get; set;} //敌人检测玩家的距离.
        
        public virtual float maxBumpXForce { get; set;}//敌人死后生成的金币的水平抛出力的最大值.
        public virtual float minBumpYForce { get; set;}//敌人死后生成的金币的垂直抛出力的最小值.
        public virtual float maxBumpYForce { get; set;}//敌人死后生成的金币的垂直抛出力的最大值.
        
       
        
        
    }
    
    [Serializable]
    public class CrawlerParameter : Parameter
    {
        public override int health { get; set;} = 3;
        public override int damage { get; set;} = 1;
        public override float moveSpeed { get; set;} = 3f;
        
        public override float maxBumpXForce { get; set;} = 10;
        public override float minBumpYForce { get; set;} = 3;
        public override float maxBumpYForce { get; set;} = 5;
        
        public float hurtForce { get; set;} = 5f; //受伤时的击退力度
        public float deadForce { get; set;} = 8f;
        public int minSpawnCoins { get; set;} = 2;//敌人死后生成的金币数量的最小值.
        public int maxSpawnCoins { get; set;} = 5;//敌人死后生成的金币数量的最大值.
    }
    
    public class GruzMotherParameter : Parameter
    {
        public override int health { get; set;} = 18;
        public override int damage { get; set;} = 1;
        public override float maxBumpXForce { get; set; } = 13;
        public override float minBumpYForce { get; set; } = 10;
        
        public override float maxBumpYForce { get; set;} = 7;
        public override float moveSpeed { get; set;} = 5f;
        
        public float crashSpeed { get; set;} = 25f;
        public float mutiCrashSpeed { get; set;} = 35f;
        
        public float deadForce { get; set;} = 8f;
        
        public int coins {get; set;} = 50;
        
        public float detectDistance { get; set;} = 10f;
    }
    
    public class GruzParameter : Parameter
    {
        public override int health { get; set;} = 2;
        public override int damage { get; set;} = 1;
        public override float moveSpeed { get; set;} = 3f;
        
        public override float detectDistance { get; set;} = 10f;

        public float hurtForcce { get; set; } = 4f;
        public float deadForce { get; set;} = 4f;
        
        public int coins {get; set;} = 50;
    }
    
    public class VengeflyParameter : Parameter
    {
        public override int health { get; set;} = 2;
        public override int damage { get; set;} = 1;
        public override float moveSpeed { get; set;} = 4f;
        
        public override float detectDistance { get; set;} = 10f;
        public override float maxBumpXForce { get; set;} = 10;
        public override float minBumpYForce { get; set;} = 3;
        public override float maxBumpYForce { get; set;} = 5;

        public float hurtForcce { get; set; } = 4f;
        public float deadForce { get; set;} = 4f;
        
        public int minSpawnCoins { get; set;} = 2;//敌人死后生成的金币数量的最小值.
        public int maxSpawnCoins { get; set;} = 5;//敌人死后生成的金币数量的最大值.
    }
}
