### Feedback

*Please add below any feedback you want to send to the team*

#### Note: For the scope of this challenge I've decided to keep things simple and have made some tradeoffs

I've created a new docker compose file named ***docker-compose-with-challenge.yaml*** to run the cinema api with all the dependencies.
You can use it.
A swagger page is available here: https://localhost:7235/swagger/index.html

All the required features has been implemented and I've fixed the proved api configurations issues

#### Known issues and tradeoffs in my implementation:

- Ideally we should make sure the auditorium is available for the movie duration,
i.e there is no scheduled show during the session date and the session date + movie duration


- Concurrency during seats reservation: the current implementation of the seats reservation does not handle concurrency and we may have some race conditions:
  In a single database environment we can implement 2 locking strategies:
    - Optimistic locking which consists of reading the state of the row at the beginning and when ready to do the transaction, we make sure the row version has not changed otherwise we cancel the transaction and resolve the conflict . 
    - Pessimistic locking where we lock the resource(seat) before doing the transaction. It's highly consistent but can slow down the reservation process
    - To handle reservation expiration, I could have used redis by locking seats for 10 min.
  - In a distributed environment, we can use a distributed locking system like redis
  

- I've changed the DBContext scope registration to transient in order to avoid issues with already tracked entities
- I haven't implemented dedicated DTOs to prevent exposing private data through the endpoints. This is a bad practice.
- For tests, not every cases/functionalities have been tested as it takes time.