Solo Presentation: July 20th
    
    3. Refactor endpoints to better utilize OOP concepts        
        - keep BudgetAPI as base class that establishes the variables, maintains
        the view totalExpenses & viewDetail methods, and have the other two
        classes inherit to avoid having to re-establish the variables



    4. Ask user for what command they want to give
        - Give a set of possible commands; automatically convert all responses to lower
        - if a typo or unavailable option, rerun Console.ReadLine();
        - if null, give another response and rerun Console.ReadLine();

        - With given command, create http request

--------------------------------------------------------------------------------

    Further functionality:
    - Current monthly expenditure & how much we have left to spend
    - Categories
    - Simple auth (fail to login, shut down), transactions only with given name;

    Keep all console interactions seperate, into its own class perhaps?
        - Print to console, but also return to user? (postman)
        - Look @ banking app with string builder to make a clean post to console
    
    Set a savings goal; send email if go above it

    Fix null reference warnings


    