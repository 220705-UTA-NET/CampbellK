# Project 1

**Tasks**

2. Establish routes in ASP.net Application
    - Controllers directory 
        -- Should call the CRUD functions from Data, do something with it, and
        return something
        
        -- Review vocab (FetchAllFlashcards)
        -- See a ledger of all words (FetchAllFlashcards)
        -- Create a new word (CreateNewCard)
        -- Editing existing card (EditCard)
        -- Delete a card (DeleteCard)
        -- Delete all cards (DeleteAllCards)



    - Middleware directory

    - Routes directory

3. Create UserInput interface in Console Application
    - Display word, have user type the definition, then display correct 
    definition, sentence. allow for early exit
        ++ should be a method within Flashcard class

--------------------------------------------------------------------------------

**Idea**
1. Low-budget Anki copy that saves to Azure
    - Data: auto id, vocab word, definition, furigana, example sentence, 
        difficulty
    - Be able to make cards, edit cards, & review the flash cards
    - Be able to mark how well you know the word


2. Is there an API with Japanese vocabulary that I can auto-generate vocab?


3. Use case for Google translate API?


4. Incorporate a user table with simple login?


--------------------------------------------------------------------------------

**Presentation Date: August 2nd (Tuesday)**

Requirements:
1. Need a user interface (a console application)
    - UI will communicate with API via HTTP

2. Need an ASP.net web api service
    - Communicates to DB with ADO.net (SqlConnection)

3. SQL Database persistance (Azure)



--------------------------------------------------------------------------------
