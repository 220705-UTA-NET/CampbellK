-- DROP TABLE flashcards;

-- CREATE TABLE flashcards (
--     Id int IDENTITY PRIMARY KEY,
--     Word nvarchar(50) NOT NULL,
--     Definition nvarchar(100) NOT NULL,
--     Example text,
--     Notes text,
--     Difficulty nvarchar(10)
-- );

INSERT INTO flashcards (Word, Definition, Example, Notes, Difficulty) VALUES ('hello', 'world', 'example', 'notes example', 'easy');

SELECT * FROM flashcards;