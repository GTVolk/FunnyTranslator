using System;

namespace FunnyTranslator
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger logger = new Logger();
            if (args.Length == 2)
            {
                // Object creation
                LangProcessor processor = new LangProcessor();
                LexicalAnalyzer lexAnalyzer = new LexicalAnalyzer(processor, logger);
                SyntaxAnalyzer synAnalyzer = new SyntaxAnalyzer(processor, logger);
                FileWorker fileProcessor = new FileWorker(args[0]);

                // Loading program data
                processor.lexical.Alphabet.loadFromFile("data\\alphabet.dat");
                processor.lexical.LexicalTable.loadFromFile("data\\lextable.dat");
                processor.lexical.AllowedStates.loadFromFile("data\\allowedstates.dat");

                processor.syntax.Actions.loadFromFile("data\\actions.dat");
                processor.syntax.KeyWords.loadFromFile("data\\keywords.dat");
                processor.syntax.LexIndexes.loadFromFile("data\\lexindexes.dat");
                processor.syntax.SyntaxTable.loadFromFile("data\\syntable.dat");
                

                // Translate
                lexAnalyzer.parse(fileProcessor.readAllToString());
                logger.Log("Lexical parse completed");
                int errors = 0;
                if ((errors = lexAnalyzer.Errors) > 0)
                {
                    logger.Log("Unable to continue translation. There are " + errors + " lexical errors in program.");
                    Console.ReadKey();
                }
                else
                {
                    synAnalyzer.parse(lexAnalyzer.LexicalTable);
                    if ((errors = synAnalyzer.getErrorsCount()) > 0)
                    {
                        logger.Log("Unable to continue translation. There are " + errors + " syntax errors in program.");
                        Console.ReadKey();
                    }
                    else
                        FileWorker.writeToFile(synAnalyzer.getTranslatedData(), args[1]);
                }
                lexAnalyzer.clearLex();
            }
            else
                logger.Log("Invalid translator args!\n\tUsing: FunnyTranslator <filename>,\n\t\tfilename - file transltating to pascal language");
        }
    }
}
