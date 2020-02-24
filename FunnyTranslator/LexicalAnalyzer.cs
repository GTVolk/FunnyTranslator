using System.Collections.Generic;

namespace FunnyTranslator
{
    class LexicalAnalyzer
    {
        private LangProcessor processor;
        private Logger logger;

        private LStack<lexChain> lexTable;

        private string inputChain;
        private int errors;

        public LexicalAnalyzer(LangProcessor processor, Logger logger)
        {
            this.processor = processor;
            this.logger = logger;

            lexTable = new LStack<lexChain>();

            this.errors = 0;
            this.inputChain = "";
        }

        public int Errors
        {
            get { return this.errors; }
        }

        public LStack<lexChain> LexicalTable
        {
            get { return new LStack<lexChain>(lexTable); }
        }

        private void pushLex(lexChain lex)
        {
            this.lexTable.push(lex);
        }

        public void clearLex()
        {
            this.lexTable.Clear();
        }

        private char getChar(int i)
        {
            return this.inputChain[i];
        }

        private string charToString(char symbol)
        {
            return new string(new char[] { symbol });
        }

        private bool isFilled(int i)
        {
            return (i < this.inputChain.Length);
        }

        private bool isFirstEqual(string data, List<char> cl)
        {
            bool res = false;
            if (data.Length > 0)
                foreach (char c in cl)
                    res |= data[0] == c;
            return res;
        }

        public void parse(string data)
        {
            this.logger.Log("Lexical analyzer started...");

            this.inputChain = data;
            List<char> strquote = new List<char>() { '\'', '\"' };

            this.clearLex();

            lexChain lecs;

            int i = 0, state = FunnyTranslator.LexicalTable.START_STATE, line = 0;
            string chain = "";

            while (this.isFilled(i))
            {
                char symbol = this.getChar(i);
                string ch = this.charToString(symbol);
                int oldState = state;

                if (ch == "\n")
                    line++;

                if (!((ch == "\n") || ((ch == " ") && (!this.isFirstEqual(chain,strquote))) || (ch == "\t") || (ch == "\r")))
                {
                    int sGroup = this.processor.lexical.Alphabet.getGroup(symbol);
                    if ((sGroup != Alphabet.UNDEFINED_GROUP) && (state != FunnyTranslator.LexicalTable.UNDEFINED_STATE))
                        state = this.processor.lexical.LexicalTable[sGroup, state];
                    if ((state != FunnyTranslator.LexicalTable.UNDEFINED_STATE) && this.isFilled(i + 1))
                        chain += symbol;
                    else
                    {
                        if (!this.isFilled(i + 1))
                        {
                            chain += symbol;
                            oldState = state;
                        }
                        if (this.processor.lexical.AllowedStates.isAllowedState(oldState))
                        {
                            if (!(lecs = this.processor.createLex(chain, oldState, line)).isNull())
                                this.pushLex(lecs);
                            state = this.processor.lexical.LexicalTable[this.processor.lexical.Alphabet.getGroup(symbol), FunnyTranslator.LexicalTable.START_STATE];
                        }
                        else
                        {
                            this.errors++;
                            this.logger.Log("error on " + line + " line! " + chain + " is illegal idintifier");
                            sGroup = this.processor.lexical.Alphabet.getGroup(symbol);
                            state = FunnyTranslator.LexicalTable.START_STATE;
                            if (sGroup != Alphabet.UNDEFINED_GROUP)
                                state = this.processor.lexical.LexicalTable[sGroup, FunnyTranslator.LexicalTable.START_STATE];
                        }
                        chain = "" + symbol;
                    }
                }
                else
                {
                    if (this.processor.lexical.AllowedStates.isAllowedState(oldState) || (chain == ""))
                    {
                        if ((chain != "") && !(lecs = this.processor.createLex(chain, oldState, line)).isNull())
                            this.pushLex(lecs);
                    }
                    else
                    {
                        this.errors++;
                        this.logger.Log("error on " + line + " line! " + chain + " is illegal idintifier");
                    }
                    chain = "";
                    state = FunnyTranslator.LexicalTable.START_STATE;
                }
                i++;
            }
        }
    }
}
