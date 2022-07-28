-- DROP TABLE flashcards;
-- DROP TABLE review;

-- CREATE TABLE flashcards (
--     Id int IDENTITY PRIMARY KEY,
--     Word nvarchar(50) NOT NULL UNIQUE,
--     Definition nvarchar(100) NOT NULL,
--     Example ntext,
--     Reading ntext,
--     Difficulty nvarchar(10)
-- );

-- CREATE TABLE review (
--     Id int IDENTITY PRIMARY Key,
--     Word nvarchar(50),
--     SuccessfulReviews int,
--     FailedReviews int,
-- );

-- ALTER TABLE flashcards
-- ADD FOREIGN KEY (Word) REFERENCES review(Word)

-- INSERT INTO review (Word, SuccessfulReviews, FailedReviews) VALUES ('test', 0, 0);
-- INSERT INTO flashcards (Word, Definition, Example, Reading, Difficulty) VALUES ('test', 'test', 'test', 'test', 'test');

SELECT * FROM flashcards;
SELECT * FROM review;