using System;

namespace FunnyTranslator
{
    class LangProcessor
    {
        private LexicalProcessor lexProcessor;
        private SyntaxProcessor synProcessor;

        public LangProcessor()
        {
            this.lexProcessor = new LexicalProcessor();
            this.synProcessor = new SyntaxProcessor();
        }

        public LexicalProcessor lexical
        {
            get { return this.lexProcessor; }
        }

        public SyntaxProcessor syntax
        {
            get { return this.synProcessor; }
        }

        public lexChain createLex(string chain, int state, int line)
        {
            int ind = state, i2;
            if ((ind == LexIndexes.IdentifierState) && ((i2 = this.syntax.KeyWords.getKeyWordIndex(chain)) > 0))
                ind = i2;
            if (ind != LexIndexes.CommentState)
                return new lexChain(chain, this.syntax.LexIndexes[ind].i, line);
            return new lexChain();
        }

        public void pushActionsToGlobalStack(int action)
        {
            foreach (ActionValue av in this.syntax.Actions[action].data)
                this.syntax.GlobalStack.push(av);
        }
    }
}
