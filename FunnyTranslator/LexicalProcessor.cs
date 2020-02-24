using System;

namespace FunnyTranslator
{
    class LexicalProcessor
    {
        private Alphabet alphabet;
        private LexicalTable lexTable;
        private AllowedStates allowStates;

        public LexicalProcessor()
        {
            this.alphabet = new Alphabet();
            this.lexTable = new LexicalTable();
            this.allowStates = new AllowedStates();
        }

        public Alphabet Alphabet
        {
            get { return this.alphabet; }
        }

        public LexicalTable LexicalTable
        {
            get { return this.lexTable; }
        }

        public AllowedStates AllowedStates
        {
            get { return this.allowStates; }
        }
    }
}
