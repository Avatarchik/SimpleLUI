using NLua;
using UnityEngine;

namespace SimpleLUI
{
    public class SomeClass
    {
        public string MyProperty { get; private set; }

        public SomeClass(string param1 = "defaulValue")
        {
            MyProperty = param1;
        }

        public int Func1()
        {
            return 32;
        }

        public string AnotherFunc(int val1, string val2)
        {
            return "Some String " + val1 + " " + val2;
        }

        public static string StaticMethod(int param)
        {
            return "Return of Static Method";
        }
    }

    public class SimpleLUIMain : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            Lua state = new Lua();
            SomeClass obj = new SomeClass("Param");
            state["obj"] = obj;

            var s = state.DoString(@"
	        local res1 = obj:AnotherFunc(32, 'x')
            return res1
	        ")[0];

            Debug.Log(s);
        }


    }
}
