using Newtonsoft.Json.Linq;
using Solcery.BrickInterpretation;
using Solcery.BrickInterpretation.Conditions;
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
            
            BrickService.GetInstance().Registration(new BrickInterpretation.Values.BrickValueConst());
            BrickService.GetInstance().Registration(new BrickInterpretation.Conditions.BrickConditionConst());
            BrickService.GetInstance().Registration(new BrickInterpretation.Conditions.BrickConditionNot());
            BrickService.GetInstance().Registration(new BrickInterpretation.Conditions.BrickConditionOr());
            BrickService.GetInstance().Registration(new BrickInterpretation.Conditions.BrickConditionAnd());
            BrickService.GetInstance().Registration(new BrickInterpretation.Conditions.BrickConditionEqual());
            BrickService.GetInstance().Registration(new BrickInterpretation.Conditions.BrickConditionGreaterThan());
            BrickService.GetInstance().Registration(new BrickInterpretation.Conditions.BrickConditionLesserThan());
            
            var json = JObject.Parse(Resources.Load<TextAsset>("test").text);

            var brick = BrickService.GetInstance().Create(json.GetValue<string>("type")) as BrickCondition;
            var result = brick.Run(json.GetValue<JArray>("params"), null);
            Debug.Log($"Brick run result {result}");
        }
    }
}
