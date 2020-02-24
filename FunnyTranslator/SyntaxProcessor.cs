using System;

namespace FunnyTranslator
{
    class SyntaxProcessor
    {
        private Actions actions;
        private KeyWords keyWords;
        private LexIndexes lexIndexes;
        private SyntaxTable synTable;

        private GlobalStack glStack;

        public SyntaxProcessor()
        {
            this.actions = new Actions();
            this.keyWords = new KeyWords();
            this.lexIndexes = new LexIndexes();
            this.synTable = new SyntaxTable();

            this.glStack = new GlobalStack();
        }

        public Actions Actions
        {
            get { return this.actions; }
        }

        public KeyWords KeyWords
        {
            get { return this.keyWords; }
        }

        public LexIndexes LexIndexes
        {
            get { return this.lexIndexes; }
        }

        public SyntaxTable SyntaxTable
        {
            get { return this.synTable; }
        }

        public GlobalStack GlobalStack
        {
            get { return this.glStack; }
        }
    }
}
