using System.Collections.Generic;
using System.Linq;

namespace FunnyTranslator
{

    // COMMON DATATYPES
    struct lexChain
    {
        public string val;
        public int type;
        public int l;

        public lexChain(string val = "", int type = 0, int l = 0)
        {
            this.val = val;
            this.type = type;
            this.l = l;
        }

        public bool isNull()
        {
            if ((this.val == "") && (this.type == 0) && (this.l == 0))
                return true;
            return false;
        }
    }

    struct ActionValue
    {
        public int i;
        public int val;

        public ActionValue(int i = 0, int val = 0)
        {
            this.i = i;
            this.val = val;
        }
    }

    struct KeyValue
    {
        public string key;
        public int i;

        public KeyValue(string key = "", int i = 0)
        {
            this.key = key;
            this.i = i;
        }
    }

    struct LexData
    {
        public int state;
        public int i;
        public string type;

        public LexData(int state = 0, int i = 0, string type = "")
        { 
            this.state = state;
            this.i = i;
            this.type = type;
        }
    }

    struct ActionData
    {
        public bool shift;
        public ActionValue[] data;

        public ActionData(bool shift = false, ActionValue[] data = null)
        {
            this.shift = shift;
            this.data = data;
        }
    }

    class LStack<T> : List<T>
    {
        #region Constructors

        public LStack() : base() { }
        public LStack(int capacity) : base(capacity) { }
        public LStack(IEnumerable<T> collection) : base(collection) { }

        #endregion

        #region Methods

        public T pop()
        {
            if (this.Count < 1)
                return default(T);
            T item = this.Last();
            this.RemoveAt(this.Count - 1);
            return item;
        }

        public T peek()
        {
            if (this.Count < 1)
                return default(T);
            return this.Last();
        }

        public void push(T item)
        {
            this.Add(item);
        }

        public T shift()
        {
            if (this.Count < 1)
                return default(T);
            T item = this.First();
            this.RemoveAt(0);
            return item;
        }

        #endregion
    }
}
