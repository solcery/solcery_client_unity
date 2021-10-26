using Newtonsoft.Json.Linq;
using Solcery.BrickInterpretation;
using Solcery.BrickInterpretation.Conditions;
using Solcery.BrickInterpretation.Values;
using Solcery.Utils;
using TMPro;
using UnityEngine;

namespace Solcery
{
    public class SetText : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text text;
        
        // Start is called before the first frame update
        void Start()
        {
            text.text = "Prod";
#if DEV
            text.text = "Dev";
#endif
            
            BrickService.GetInstance().Registration(new BrickValueConst());
            BrickService.GetInstance().Registration(new BrickConditionConst());
            BrickService.GetInstance().Registration(new BrickConditionNot());
            BrickService.GetInstance().Registration(new BrickConditionOr());
            BrickService.GetInstance().Registration(new BrickConditionAnd());
            BrickService.GetInstance().Registration(new BrickConditionEqual());
            BrickService.GetInstance().Registration(new BrickConditionGreaterThan());
            BrickService.GetInstance().Registration(new BrickConditionLesserThan());
            
            var json = JObject.Parse(Resources.Load<TextAsset>("test").text);

            BrickUtils.TryGetBrickTypeName(json, out var brickTypeName);
            BrickUtils.TryGetBrickParameters(json, out var @params);
            BrickService.GetInstance().TryCreate(brickTypeName, out BrickCondition condition);
            var result = condition.Run(@params, Context.Create());
            BrickService.GetInstance().Free(condition);
            Debug.Log($"Brick run result {result}");
        }
    }
}
