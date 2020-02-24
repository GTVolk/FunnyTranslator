using System;
using System.Collections.Generic;
using System.Linq;

namespace FunnyTranslator
{
    class SyntaxAnalyzer
    {
        LangProcessor processor;
        Logger logger;

        private LStack<lexChain> lexTable;
        private LStack<lexChain> tmpLexTable;

        private LStack<string> identifierStack;

        private LStack<string> arifmeticStack;
        private LStack<string> labelStack;
        private LStack<string> logicalStack;  

        private List<string> arifmeticVariables;
        private List<string> labelVariables;
        private List<string> logicVariables;
        

        private string lastTerminal;
        private LStack<string> minus;

        private string compiledProgram;
        private int errors;

        public SyntaxAnalyzer(LangProcessor processor, Logger logger)
        {
            this.processor = processor;
            this.logger = logger;

            this.lexTable = new LStack<lexChain>();
            this.tmpLexTable = new LStack<lexChain>();

            this.identifierStack = new LStack<string>();

            this.arifmeticStack = new LStack<string>();
            this.labelStack = new LStack<string>();
            this.logicalStack = new LStack<string>();
            
            this.arifmeticVariables = new List<string>();
            this.labelVariables = new List<string>();
            this.logicVariables = new List<string>();
            
            this.minus = new LStack<string>();

            this.compiledProgram = "";
            this.errors = 0;
        }

        public void pop_shift()
        {
            this.tmpLexTable.push(lexTable.shift());
        }

        public void pop_keep()
        {
            this.tmpLexTable.push(this.lexTable[0]);
        }

        public void doAction(int action)
        {
            if (action == Actions.START_ACTION)
                this.lastTerminal = this.lexTable[0].val;
            this.processor.pushActionsToGlobalStack(action);
            if (action == Actions.PLUS_ACTION)
                this.minus.push(" ");
            if (action == Actions.MINUS_ACTION)
                this.minus.push("-");
            if (this.processor.syntax.Actions.isShift(action))
                this.pop_shift();
            else
                this.pop_keep();
        }

        private void compileAdd(string str)
        {
            this.compiledProgram += str;
        }

        public void translateActionSymbol(int symbol)
        {
            switch (symbol)
            {
                case 0 :
                case 2:
                case 5: { this.compileAdd(this.tmpLexTable.Last().val + "\n"); break; }
                case 1 : { this.compileAdd(" : INTEGER;\n"); break; }
                case 3 : { this.compileAdd("END."); break; }
                case 4 : { this.compileAdd(this.tmpLexTable.pop().val); this.tmpLexTable.Clear(); break; }
                case 6 : { this.compileAdd(";\n"); break; }
                case 7 : { this.compileAdd("WRITE(" + this.tmpLexTable[this.tmpLexTable.Count - 4].val + ");\n");
                           this.compileAdd("READLN(" + this.tmpLexTable[this.tmpLexTable.Count - 2].val + ")"); break; }
                case 8 : { this.compileAdd("WRITE(" + this.tmpLexTable[this.tmpLexTable.Count - 4].val + ");\n");
                           this.compileAdd("WRITELN(" + this.tmpLexTable[this.tmpLexTable.Count - 2].val + ")"); break; }
                case 9 : { this.identifierStack.push(this.tmpLexTable[this.tmpLexTable.Count - 1].val); break; }
                case 10 : { this.compileAdd(this.identifierStack.pop() + " := " + this.arifmeticStack.pop()); break; }
                case 11 : { string label = "__specialLabel" + this.labelVariables.Count;
                            this.compileAdd("IF NOT(" + this.logicalStack.pop() + ") THEN GOTO " + label + ";\n");
                            this.labelStack.push(label); this.labelVariables.Add(label); break; }
                case 12 : { string label = "__specialLabel" + this.labelVariables.Count;
                            this.compileAdd(";\nGOTO " + label + ";\n" + this.labelStack.pop() + ":\n");
                            this.labelStack.push(label); this.labelVariables.Add(label); break; }
                case 13 : { this.compileAdd(";\n" + this.labelStack.pop() + ":\n"); break; }
                case 14 :
                case 15 : { this.arifmeticStack.push(this.tmpLexTable[this.tmpLexTable.Count - 1].val); break; }
                case 16 : 
                case 17 : { string newVar = " __arifmeticVar" + this.arifmeticVariables.Count;
                            string r2 = this.arifmeticStack.pop(); string r1 = this.arifmeticStack.pop();
                            this.arifmeticVariables.Add(newVar); this.arifmeticStack.push(newVar);
                            string m1 = "", m2 = "";
                            if (this.minus.Count > 1)
                                m1 = this.minus[this.minus.Count - 2];
                            if (this.minus[this.minus.Count - 1] != "*")
                                m2 = this.minus[this.minus.Count - 1];
                            this.minus.pop();
                            if (minus.Count > 0) this.minus[this.minus.Count - 1] = "*";
                            this.compileAdd(newVar + " := " + m1 + r1 + " + " + m2 + r2 + ";\n"); break; }
                case 18 : { string newVar = "__logicalVar" + this.logicVariables.Count;
                            string r2 = this.logicalStack.pop(); string r1 = this.logicalStack.pop();
                            this.logicVariables.Add(newVar); this.logicalStack.push(newVar);
                            this.compileAdd(newVar + " := " + r1 + " or " + r2 + ";\n"); break; }
                case 19 : { string newVar = "__logicalVar" + this.logicVariables.Count;
                            string r2 = this.logicalStack.pop(); string r1 = this.logicalStack.pop();
                            this.logicVariables.Add(newVar); this.logicalStack.push(newVar);
                            this.compileAdd(newVar + " := " + r1 + " and " + r2 + ";\n"); break; }
                case 20 : { string newVar = "__logicalVar" + this.logicVariables.Count;
                            string r1 = this.logicalStack.pop();
                            this.logicVariables.Add(newVar); this.logicalStack.push(newVar);
                            this.compileAdd(newVar + " := not " + r1 + ";\n"); break; }
                case 21 : { string newVar = "__logicalVar" + this.logicVariables.Count;
                            string r2 = this.arifmeticStack.pop(); string r1 = this.arifmeticStack.pop();
                            this.logicVariables.Add(newVar); this.logicalStack.push(newVar);
                            this.compileAdd(newVar + " := " + r1 + this.lastTerminal + r2 + ";\n"); break; }
                default : { break; }
            }
        }

        public string getTranslatedData()
        {
            return this.compiledProgram;
        }

        public int getErrorsCount()
        {
            return this.errors;
        }

        public void parse(LStack<lexChain> lexTable)
        {
            logger.Log("Syntax analyzer started...");
            this.processor.syntax.GlobalStack.init();
            this.compiledProgram = "";
            this.lexTable = lexTable;
            this.tmpLexTable.Clear(); this.identifierStack.Clear();
            this.arifmeticStack.Clear(); this.arifmeticVariables.Clear();
            this.logicalStack.Clear();  this.logicVariables.Clear();     
            this.labelStack.Clear(); this.labelVariables.Clear();
            this.minus.Clear();
            int action = 0;
            ActionValue glStackTop;
            int stackSymbol;

            while ((this.processor.syntax.GlobalStack.Size != 0) && (this.lexTable.Count != 0) && (action != Actions.UNDEFINED_ACTION))
            {
                glStackTop = this.processor.syntax.GlobalStack.pop();
                if (glStackTop.i != Actions.SYNTAX_ACTION)
                {
                    stackSymbol = glStackTop.i;
                    if (stackSymbol == Actions.SYNTAX_SYMBOL)
                        stackSymbol = glStackTop.val;
                    action = this.processor.syntax.SyntaxTable[stackSymbol, this.lexTable[0].type];      
                    if (action != -1)
                        this.doAction(action);
                }
                else
                    this.translateActionSymbol(glStackTop.val);
                if ((this.processor.syntax.GlobalStack.Size == 1) && (this.lexTable.Count == 0))
                {
                    this.translateActionSymbol(this.processor.syntax.GlobalStack.bottom().val);  
                    this.processor.syntax.GlobalStack.pop();
                }
            }
            string[] spl = this.compiledProgram.Split(new string[] { "VAR" }, StringSplitOptions.None);
            if (this.labelVariables.Count > 0)
            {
                spl[0] = "LABEL ";
                for (var i = 0; i < this.labelVariables.Count - 1; i++)
                    spl[0] += "\n" + this.labelVariables[i] + ",";
                spl[0] += "\n" + this.labelVariables[this.labelVariables.Count - 1] + ";\nVAR";
            }
            else
                spl[0] = "VAR";
            if (this.logicVariables.Count > 0)
            {
                for (int i = 0; i < this.logicVariables.Count - 1; i++)
                    spl[0] += "\n" + this.logicVariables[i] + ",";
                spl[0] += "\n" + this.logicVariables[this.logicVariables.Count - 1] + " :boolean;\n";
            }
            for (int i = 0; i < this.arifmeticVariables.Count; i++)
                spl[0] += "\n" + this.arifmeticVariables[i] + ",";
            if (spl.Length > 1)
                this.compiledProgram = spl[0] + spl[1];
            if (action == Actions.UNDEFINED_ACTION)
            {
                logger.Log("\n^Error: unexpected symbol on the " + this.lexTable[0].l + " line:" + this.lexTable[0].val + " \n");
                this.errors++;
            }
            else
                if ((this.processor.syntax.GlobalStack.Size != 0) && (this.lexTable.Count == 0))
                {
                    logger.Log("\n^Error: unexpected end of the file\n");
                    this.errors++;
                }
                else
                    if ((this.processor.syntax.GlobalStack.Size == 0) && (this.lexTable.Count != 0))
                        logger.Log("\n^Warning: there code after end.'\n");
                    else
                        logger.Log("Translation complete!\n");
        }
    }
}
