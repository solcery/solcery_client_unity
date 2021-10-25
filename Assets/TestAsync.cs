using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Solcery.Utils;
using UnityEngine;

namespace Solcery
{
    public enum EntityTypes
    {
        empty,
        value_bool,
        value_int,
        value_string,
        brick_value_const,
        brick_condition_const,
        brick_condition_not,
        brick_condition_equal,
        brick_condition_greater_than,
        brick_condition_lesser_than,
        brick_condition_and,
        brick_condition_or
    }
    
    public abstract class EntityData
    {
        public readonly EntityTypes EntityType;

        public static EntityData Parse(JObject json)
        {
            return json.GetEnum<EntityTypes>("type") switch
            {
                EntityTypes.value_bool => ValueData.Parse(json),
                EntityTypes.value_int => ValueData.Parse(json),
                EntityTypes.value_string => ValueData.Parse(json),
                EntityTypes.brick_value_const => BrickData.Parse(json),
                EntityTypes.brick_condition_and => BrickData.Parse(json),
                EntityTypes.brick_condition_const => BrickData.Parse(json),
                EntityTypes.brick_condition_equal => BrickData.Parse(json),
                EntityTypes.brick_condition_greater_than => BrickData.Parse(json),
                EntityTypes.brick_condition_lesser_than => BrickData.Parse(json),
                EntityTypes.brick_condition_not => BrickData.Parse(json),
                EntityTypes.brick_condition_or => BrickData.Parse(json),
                _ => EmptyData.Create()
            };
        }

        protected EntityData(EntityTypes entityType)
        {
            EntityType = entityType;
        }
    }

    public class EmptyData : EntityData
    {
        public static EntityData Create()
        {
            return new EmptyData();
        }
        
        private EmptyData() : base(EntityTypes.empty) { }
    }


    public abstract class ValueData : EntityData
    {
        public new static EntityData Parse(JObject json)
        {
            return json.GetEnum<EntityTypes>("type") switch
            {
                EntityTypes.value_bool => BoolValueData.Create(json.GetValue<bool>("value")),
                EntityTypes.value_int => IntValueData.Create(json.GetValue<int>("value")),
                EntityTypes.value_string => StringValueData.Create(json.GetValue<string>("value")),
                _ => EmptyData.Create()
            };
        }
        
        protected ValueData(EntityTypes entityType) : base(entityType) { }
    }

    public class BoolValueData : ValueData
    {
        public readonly bool Value;

        public static BoolValueData Create(bool value)
        {
            return new BoolValueData(value);
        }
        
        private BoolValueData(bool value) : base(EntityTypes.value_bool)
        {
            Value = value;
        }
    }

    public class IntValueData : ValueData
    {
        public readonly int Value;
        
        public static IntValueData Create(int value)
        {
            return new IntValueData(value);
        }
        
        private IntValueData(int value) : base(EntityTypes.value_int)
        {
            Value = value;
        }
    }

    public class StringValueData : ValueData
    {
        public readonly string Value;
        
        public static StringValueData Create(string value)
        {
            return new StringValueData(value);
        }
        
        private StringValueData(string value) : base(EntityTypes.value_string)
        {
            Value = value;
        }
    }

    public class BrickData : EntityData
    {
        public readonly List<EntityData> Parameters;

        public new static BrickData Parse(JObject json)
        {
            var result = new BrickData(json.GetEnum<EntityTypes>("type"));
            var paramsArray = json.GetValue<JArray>("params");
            foreach (var paramToken in paramsArray)
            {
                if (paramToken is JObject paramObject)
                {
                    result.AddParameter(EntityData.Parse(paramObject));
                }
            }
            return result;
        }

        public static BrickData Create(EntityTypes entityType)
        {
            return new BrickData(entityType);
        }

        private BrickData(EntityTypes entityType) : base(entityType)
        {
            Parameters = new List<EntityData>();
        }

        public void AddParameter(EntityData parameter)
        {
            Parameters.Add(parameter);
        }
    }

    public static class BrickFabric
    {
        public static Brick Create(BrickData parameter)
        {
            switch (parameter.EntityType)
            {
                case EntityTypes.brick_value_const:
                    return new BrickValueConst();
                
                case EntityTypes.brick_condition_const:
                    return new BrickConditionConst();
                
                case EntityTypes.brick_condition_not:
                    return new BrickConditionNot();
                
                case EntityTypes.brick_condition_and:
                    return new BrickConditionalAnd();
                
                case EntityTypes.brick_condition_or:
                    return new BrickConditionalOr();
                
                case EntityTypes.brick_condition_equal:
                    return new BrickConditionEqual();
                
                case EntityTypes.brick_condition_greater_than:
                    return new BrickConditionalGreaterThan();
                
                case EntityTypes.brick_condition_lesser_than:
                    return new BrickConditionalLesserThan();
            }
            
            return null;
        }
    }
    
    public interface IContext {}

    public abstract class Entity
    {
        
    }

    public abstract class Brick : Entity
    {
        
    }

    public abstract class Value : Brick
    {
        public abstract int Run(IReadOnlyList<EntityData> parameters, IContext context);
    }

    public class BrickValueConst : Value
    {
        public override int Run(IReadOnlyList<EntityData> parameters, IContext context)
        {
            if (parameters.Count > 0 
                && parameters[0] is IntValueData intValueData)
            {
                return intValueData.Value;
            }
            
            throw new ArgumentException("parameters[0] is not IntValueData!");
        }
    }

    public abstract class Condition : Brick
    {
        public abstract bool Run(IReadOnlyList<EntityData> parameters, IContext context);
    }

    public class BrickConditionConst : Condition
    {
        public override bool Run(IReadOnlyList<EntityData> parameters, IContext context)
        {
            if (parameters.Count > 0 
                && parameters[0] is BoolValueData boolValueData)
            {
                return boolValueData.Value;
            }

            throw new ArgumentException("parameters[0] is not BoolValueData!");
        }
    }

    public class BrickConditionNot : Condition
    {
        public override bool Run(IReadOnlyList<EntityData> parameters, IContext context)
        {
            if (parameters.Count > 0 
                && parameters[0] is BrickData brickData 
                && BrickFabric.Create(brickData) is Condition condition)
            {
                return !condition.Run(brickData.Parameters, context);
            }

            throw new Exception("BrickConditionNot Run has error!");
        }
    }

    public class BrickConditionEqual : Condition
    {
        public override bool Run(IReadOnlyList<EntityData> parameters, IContext context)
        {
            if (parameters.Count > 1 
                && parameters[0].TryToData(out BrickData brickData1)
                && parameters[0].TryToData(out BrickData brickData2) 
                && BrickFabric.Create(brickData1).TryToBrick(out Value value1)
                && BrickFabric.Create(brickData2).TryToBrick(out Value value2))
            {
                return value1.Run(brickData1.Parameters, context) == value2.Run(brickData2.Parameters, context);
            }
            
            throw new Exception("BrickConditionEqual Run has error!");
        }
    }

    public class BrickConditionalGreaterThan : Condition
    {
        public override bool Run(IReadOnlyList<EntityData> parameters, IContext context)
        {
            if (parameters.Count > 1 
                && parameters[0].TryToData(out BrickData brickData1)
                && parameters[0].TryToData(out BrickData brickData2) 
                && BrickFabric.Create(brickData1).TryToBrick(out Value value1)
                && BrickFabric.Create(brickData2).TryToBrick(out Value value2))
            {
                return value1.Run(brickData1.Parameters, context) >= value2.Run(brickData2.Parameters, context);
            }
            
            throw new Exception("BrickConditionalGreaterThan Run has error!");
        }
    }
    
    public class BrickConditionalLesserThan : Condition
    {
        public override bool Run(IReadOnlyList<EntityData> parameters, IContext context)
        {
            if (parameters.Count > 1 
                && parameters[0].TryToData(out BrickData brickData1)
                && parameters[0].TryToData(out BrickData brickData2) 
                && BrickFabric.Create(brickData1).TryToBrick(out Value value1)
                && BrickFabric.Create(brickData2).TryToBrick(out Value value2))
            {
                return value1.Run(brickData1.Parameters, context) <= value2.Run(brickData2.Parameters, context);
            }
            
            throw new Exception("BrickConditionalLesserThan Run has error!");
        }
    }
    
    public class BrickConditionalAnd : Condition
    {
        public override bool Run(IReadOnlyList<EntityData> parameters, IContext context)
        {
            foreach (var parameter in parameters)
            {
                if (parameter is BrickData brickData 
                    && BrickFabric.Create(brickData).TryToBrick(out Condition condition))
                {
                    if (!condition.Run(brickData.Parameters, context))
                    {
                        return false;
                    }
                }
                else
                {
                    throw new Exception("BrickConditionalAnd Run has error!");
                }
            }

            return true;
        }
    }
    
    public class BrickConditionalOr : Condition
    {
        public override bool Run(IReadOnlyList<EntityData> parameters, IContext context)
        {
            foreach (var parameter in parameters)
            {
                if (parameter is BrickData brickData 
                    && BrickFabric.Create(brickData).TryToBrick(out Condition condition))
                {
                    if (condition.Run(brickData.Parameters, context))
                    {
                        return true;
                    }
                }
                else
                {
                    throw new Exception("BrickConditionalAnd Run has error!");
                }
            }

            return false;
        }
    }

    public static class BrickUtils
    {
        public static bool TryToData<T>(this EntityData entityData, out T data) where T : EntityData
        {
            data = null;
            
            if (entityData is T result)
            {
                data = result;
            }

            return data != null;
        }

        public static bool TryToBrick<T>(this Brick brick, out T cBrick) where T : Brick
        {
            cBrick = null;

            if (brick is T result)
            {
                cBrick = result;
            }

            return cBrick != null;
        }
    }

    public class TestAsync : MonoBehaviour
    {
        private void Start()
        {
            // var json = JObject.Parse(Resources.Load<TextAsset>("test").text);
            // var brickData = BrickData.Parse(json);
            // var brick = BrickFabric.Create(brickData) as Condition;
            // var result = brick.Run(brickData.Parameters, null);
            // Debug.Log($"Brick run result {result}");
        }
    }
}