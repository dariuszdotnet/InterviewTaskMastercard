EXPLANATION OF READONLY RECORD STRUCT CHOICE
After research, I chose a readonly record struct to represent king objects because:
    It is a value type, stored on the stack, making it more performant than a class or record in this case.
    It is immutable, which provides memory optimizations, ensures thread safety, and eliminates the need for re-validation.
I also considered using a normal struct, but I encountered issues during implementation due to incorrect property initialization during JSON deserialization.

FURTHER STEPS IF IT WERE PRODUCTION CODE
For the sake of simplicity, I also skipped some of my typical coding style parts, including:
    Extend Unit testing
    Logging
    External configuration (e.g., here the URL is hardcoded)
    Caching kings data (for demo could be into a file). If cached data is older than 15 minutes , then re-fetch it.

ASSUMPTIONS
Since a detailed specification was not provided, I made the following assumptions:
    A.The provided years are inclusive, meaning the ruling duration is calculated as EndYear - StartYear + 1.
        Example: A ruling duration from 1001 to 1003 counts as 3 years.
    B. King IDs start from 1.
    C. The country field may be null. This does not affect the results in the current task, but we could add validation later if needed.
    D. Years can be negative (BCE dates), so the only validation criteria is that the ruling period must be positive.
    E. Data set does not provide recent change, thus Elizabeth II ruling years are calculated like she would be still ruling.
    F. I check the years results - StartYear and EndYear - after parsing has been done. This way I validate both - fetching proper data and parsing at once. I can be sure it is always parsed due to properties initialization.

BTW: I prefer a one-liner syntax, regardless of line length. However, I want to make it clear that I am happy to align with the team's coding style.