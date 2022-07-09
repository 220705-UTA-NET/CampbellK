Solo Presentation: July 20th

Create simple budget CRUD app API that interfaces with CLI
Take inputs from users for various things via Console.ReadLine();
    
    2. Create endpoints
        - Edit expense by id
        - Delete individual expense
    
    3. Refactor endpoints to better utilize OOP concepts        
        ** what will the parent type look like?
        - Can use overloading to change the route/command
            *** use an abstract or virtual base api class ***

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


    