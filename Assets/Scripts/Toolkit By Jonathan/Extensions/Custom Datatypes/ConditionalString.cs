using System;

namespace ToolkitByJonathan
{
    public class ConditionalString
    {
        private Func<bool> _condition;
        private string _textIfConditionTrue;
        private string _textIfConditionFalse;
        
        public ConditionalString(Func<bool> condition, string conditionTrueText = "", string conditionFalseText = "") {
            this._condition = condition;
            this._textIfConditionTrue = conditionTrueText;
            this._textIfConditionFalse = conditionFalseText;
        }

        public string Result()
        {
            bool isConditionMet = _condition.Invoke(); 
            string output = isConditionMet ? this._textIfConditionTrue : this._textIfConditionFalse;
            return output;
        }

        public void SetReturnTexts(string conditionTrueText = null, string conditionFalseText = null)
        {
            if (conditionTrueText != null)
                this._textIfConditionTrue = conditionTrueText;
            
            if (conditionFalseText != null)
                this._textIfConditionFalse = conditionFalseText;
        }
    }
}
