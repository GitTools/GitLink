# Setting up the solution for integration tests

GitLink has integration tests for multiple repositories. It uses NUnit and decorates the integration tests with the *Explicit* attribute to prevent it running in a CI build.

## Cloning the test repositories

GitLink uses the following remote repositories to test whether it works with actual repositories.

* https://bitbucket.org/CatenaLogic/GitLinkTestRepro
* https://github.com/CatenaLogic/GitLinkTestRepro

Below is a table with the expected remote repositories and the expected locations on disk:

* BitBucket => C:\Source\GitLinkTestRepro_BitBucket
* GitHub => C:\Source\GitLinkTestRepro_GitHub

## Running the explicit integration tests

It is required to explicitly run the integration tests that should work now.