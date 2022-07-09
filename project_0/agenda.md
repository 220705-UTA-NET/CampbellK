Solo Presentation: July 20th

Create simple budget CRUD app API that interfaces with CLI
Take inputs from users for various things via Console.ReadLine();
    
    1. Be able to read from request body for POST/PUT/DELETE

    2. Set up endpoints to read, create, edit, delete entries
        ** what will the parent type look like?
        - Can use overloading to change the route/command
            *** use an abstract or virtual base api class ***

        a. Ask user for what command they want to give
            - Give a set of possible commands; automatically convert all responses to lower
            - if a typo or unavailable option, rerun Console.ReadLine();
            - if null, give another response and rerun Console.ReadLine();

            - With given command, create http request

    3. Further functionality:
        - Current monthly expenditure & how much we have left to spend
        - Categories
        - Simple auth (fail to login, shut down), transactions only with given name;

    4. Keep all console interactions seperate, into its own class perhaps?
        - Print to console, but also return to user? (postman)
        - Look @ banking app with string builder to make a clean post to console
    
    6. Set a savings goal; send email if go above it


    