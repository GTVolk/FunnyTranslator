using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FunnyTranslator
{
    // LEXICAL DATATYPES

    class Alphabet
    {
        private const int UNDEFINED = -1;
        public const int UNDEFINED_GROUP = UNDEFINED;

        private List<string> data;

        public Alphabet()
        {
            this.data = new List<string>();
        }

        private string getItem(int i)
        {
            return this.data[i];
        }

        private void setItem(int i, string value)
        {
            this.data[i] = value;
        }

        public string this[int index]
        {
            get { return this.getItem(index); }
            set { this.setItem(index, value); }
        }

        public int Size
        {
            get { return this.data.Count; }
        }

        public void loadFromFile(string filename)
        {
            FileWorker alphabetWorker = new FileWorker(filename);
            this.data = alphabetWorker.readAllLinesToList();
        }

        public int getGroup(int symbolCode)
        {
            for (int i = 0; i < this.Size; i++)
                if (this.isAlphabetSymbol(i, symbolCode))
                    return i;
            return -1;
        }

        public bool isAlphabetSymbol(int i, int symbolCode)
        {
            return (this.getItem(i).IndexOf(new string(new char[] { (char)symbolCode })) != -1);
        }
    }

    class LexicalTable
    {
        private const int UNDEFINED = -1;

        private const int _ = UNDEFINED;
        public const int UNDEFINED_STATE = UNDEFINED;

        public const int START_STATE = 0;

        private List<int[]> table;

        public LexicalTable()
        {
            this.table = new List<int[]>();
        }

        private int[] getItem(int i)
        {
            return this.table[i];
        }

        private int getItem(int i, int j)
        {
            return this.table[i][j];
        }

        private void setItem(int i, int[] value)
        {
            this.table[i] = value;
        }

        private void setItem(int i, int j, int value)
        {
            this.table[i][j] = value;
        }

        public int[] this[int index]
        {
            get { return this.getItem(index); }
            set { this.setItem(index, value); }
        }

        public int this[int index1, int index2]
        {
            get { return this.getItem(index1, index2); }
            set { this.setItem(index1, index2, value); }
        }

        public int Size
        {
            get { return this.table.Count; }
        }

        public void loadFromFile(string filename)
        {
            FileWorker tableWorker = new FileWorker(filename);
            List<string> strings = tableWorker.readAllLinesToList();

            int columns = int.Parse(strings[0]);
            strings.RemoveAt(0);
            string regex = "[\\-\\w]+";
            Regex reg = new Regex(regex);
            foreach (string s in strings)
            {
                MatchCollection matches = reg.Matches(s);
                int[] tarray = new int[columns];
                int i = 0;
                foreach (Match m in matches)
                    tarray[i++] = (m.Value == "_" ? _ : int.Parse(m.Value));
                table.Add(tarray);
            }
        }
    }

    class AllowedStates
    {
        private const int UNDEFINED = -1;

        private const int _ = UNDEFINED;

        private List<int> data;

        public AllowedStates()
        {
            this.data = new List<int>();
        }

        private int getItem(int i)
        {
            return this.data[i];
        }

        private void setItem(int i, int value)
        {
            this.data[i] = value;
        }

        public int this[int index]
        {
            get { return this.getItem(index); }
            set { this.setItem(index, value); }
        }

        public int Size
        {
            get { return this.data.Count; }
        }

        public void loadFromFile(string filename)
        {
            FileWorker statesWorker = new FileWorker(filename);
            List<string> strings = statesWorker.readAllLinesToList();

            string regex = "[\\-\\w]+";
            Regex reg = new Regex(regex);
            MatchCollection matches = reg.Matches(strings[0]);
            foreach (Match m in matches)
                this.data.Add((m.Value == "_" ? _ : int.Parse(m.Value)));
        }

        public bool isAllowedState(int state)
        {
            if ((state < this.Size) && (state > 0))
                if (this[state] == 1)
                    return true;
            return false;
        }
    }

    // SYNTAX DATATYPES

    class Actions
    {
        private const int UNDEFINED = -1;
        public const int UNDEFINED_ACTION = UNDEFINED;

        public const int PLUS_ACTION = 15;
        public const int MINUS_ACTION = 16;

        public const int SYNTAX_ACTION = -2;
        public const int SYNTAX_SYMBOL = -3;

        public const int START_ACTION = 0;

        private List<ActionData> data;

        public Actions()
        {
            this.data = new List<ActionData>();
        }

        private ActionData getItem(int i)
        {
            return this.data[i];
        }

        private void setItem(int i, ActionData value)
        {
            this.data[i] = value;
        }

        public ActionData this[int index]
        {
            get { return this.getItem(index); }
            set { this.setItem(index, value); }
        }

        public int Size
        {
            get { return this.data.Count; }
        }

        public void loadFromFile(string filename)
        {
            FileWorker actionsWorker = new FileWorker(filename);
            List<string> strings = actionsWorker.readAllLinesToList();

            string regex = "true|false|\\(\\w+[,][\\s\\w]+\\)";
            Regex reg = new Regex(regex);
            for (int i = 0; i < strings.Count; i++)
            {
                MatchCollection matches = reg.Matches(strings[i]);
                bool shift = bool.Parse(matches[0].Value);
                ActionValue[] data = new ActionValue[matches.Count - 1];
                if (matches.Count - 1 > 0)
                {
                    for (int j = 0; j < matches.Count - 1; j++)
                    {
                        string subregex = "\\w+";
                        Regex subreg = new Regex(subregex);
                        MatchCollection submatches = subreg.Matches(matches[j + 1].Value);
                        int state = (submatches[0].Value == "sSym" ? Actions.SYNTAX_SYMBOL : (submatches[0].Value == "sAct" ? Actions.SYNTAX_ACTION : int.Parse(submatches[0].Value)));
                        int value = int.Parse(submatches[1].Value);
                        data[j] = new ActionValue(state, value);
                    }

                }
                this.data.Add(new ActionData(shift, data));
            }
        }

        public bool isShift(int action)
        {
            return this[action].shift;
        }
    }

    class KeyWords
    {
        private List<KeyValue> data;

        public KeyWords()
        {
            this.data = new List<KeyValue>();
        }

        private KeyValue getItem(int i)
        {
            return this.data[i];
        }

        private void setItem(int i, KeyValue value)
        {
            this.data[i] = value;
        }

        public KeyValue this[int index]
        {
            get { return this.getItem(index); }
            set { this.setItem(index, value); }
        }

        public int Size
        {
            get { return this.data.Count; }
        }

        public void loadFromFile(string filename)
        {
            FileWorker keywordWorker = new FileWorker(filename);
            List<string> strings = keywordWorker.readAllLinesToList();

            string regex = "[\\-\\w]+";
            Regex reg = new Regex(regex);
            for (int i = 0; i < strings.Count; i++)
            {
                MatchCollection matches = reg.Matches(strings[i]);
                int val = int.Parse(matches[0].Value);
                string key = matches[1].Value;
                this.data.Add(new KeyValue(key, val));
            }
        }

        public bool isKeyWord(int i, string chain)
        {
            return (this[i].key == chain);
        }

        public int getKeyWordIndex(string chain)
        {
            for (int i = 0; i < this.Size; i++)
                if (this.isKeyWord(i, chain))
                    return this[i].i;
            return -1;
        }
    }

    class LexIndexes
    {
        private List<LexData> data;

        private static int commentState;
        private static int identifierState;

        public LexIndexes()
        {
            this.data = new List<LexData>();

            commentState = 0;
            identifierState = 0;
        }

        private LexData getItem(int i)
        {
            return this.data[i];
        }

        private void setItem(int i, LexData value)
        {
            this.data[i] = value;
        }

        public LexData this[int index]
        {
            get { return this.getItem(index); }
            set { this.setItem(index, value); }
        }

        public int Size
        {
            get { return this.data.Count; }
        }

        public static int CommentState
        {
            get { return commentState; }
        }

        public static int IdentifierState
        {
            get { return identifierState; }
        }

        public void loadFromFile(string filename)
        {
            FileWorker keywordWorker = new FileWorker(filename);
            List<string> strings = keywordWorker.readAllLinesToList();

            string regex = "[\\-\\w]+|\".*\"";
            Regex reg = new Regex(regex);
            for (int i = 0; i < strings.Count; i++)
            {
                MatchCollection matches = reg.Matches(strings[i]);
                int state = int.Parse(matches[0].Value);
                int ind = int.Parse(matches[1].Value);
                string type = matches[2].Value;
                string comment = matches[3].Value.Replace("\"", "");
                if (type == "COMMENT") commentState = state;
                if (type == "INDENTIFIER") identifierState = state;
                this.data.Add(new LexData(state, ind, comment));
            }
        }
    }

    class SyntaxTable
    {
        private const int UNDEFINED = -1;

        private const int _ = UNDEFINED;
        public const int UNDEFINED_STATE = UNDEFINED;

        public const int START_STATE = 0;

        private List<int[]> table;

        public SyntaxTable()
        {
            this.table = new List<int[]>();
        }

        private int[] getItem(int i)
        {
            return this.table[i];
        }

        private int getItem(int i, int j)
        {
            return this.table[i][j];
        }

        private void setItem(int i, int[] value)
        {
            this.table[i] = value;
        }

        private void setItem(int i, int j, int value)
        {
            this.table[i][j] = value;
        }

        public int[] this[int index]
        {
            get { return this.getItem(index); }
            set { this.setItem(index, value); }
        }

        public int this[int index1, int index2]
        {
            get { return this.getItem(index1, index2); }
            set { this.setItem(index1, index2, value); }
        }

        public int Size
        {
            get { return this.table.Count; }
        }

        public void loadFromFile(string filename)
        {
            FileWorker tableWorker = new FileWorker(filename);
            List<string> strings = tableWorker.readAllLinesToList();

            int columns = int.Parse(strings[0]);
            strings.RemoveAt(0);
            string regex = "[\\-\\w]+";
            Regex reg = new Regex(regex);
            foreach (string s in strings)
            {
                MatchCollection matches = reg.Matches(s);
                int[] tarray = new int[columns];
                int i = 0;
                foreach (Match m in matches)
                    tarray[i++] = (m.Value == "_" ? _ : int.Parse(m.Value));
                table.Add(tarray);
            }
        }
    }

    // GLOBAL STACK

    class GlobalStack
    {
        private LStack<ActionValue> data;

        public GlobalStack()
        {
            this.data = new LStack<ActionValue>();
        }

        private ActionValue getItem(int i)
        {
            return this.data[i];
        }

        private void setItem(int i, ActionValue value)
        {
            this.data[i] = value;
        }

        public ActionValue this[int index]
        {
            get { return this.getItem(index); }
            set { this.setItem(index, value); }
        }

        public int Size
        {
            get { return this.data.Count; }
        }

        private void loadFromFile(string filename)
        {
            // NONE and PRIVATE
        }

        public void init()
        {
            this.data.Clear();
            this.data.push(new ActionValue(Actions.SYNTAX_SYMBOL, 0));
        }

        public ActionValue pop()
        {
            return this.data.pop();
        }

        public void push(ActionValue item)
        {
            this.data.push(item);
        }

        public ActionValue peek()
        {
            return this.data.peek();
        }

        public ActionValue shift()
        {
            return this.data.shift();
        }

        public ActionValue top()
        {
            return this[this.Size - 1];
        }

        public ActionValue bottom()
        {
            return this[0];
        }
    }
}
