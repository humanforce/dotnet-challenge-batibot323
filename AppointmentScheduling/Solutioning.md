# Overview
This is to run you through my thought processes as I'm solving this problem.

## Initial Thoughts
Scheduling appointments seem to be straightforward but now my initial hard think is how small I want the time resolution be. Can we assume appointments are in 15-minute blocks? Google Calendar actually allows you to specify a time down to the minute. The main problem here is to solve scheduling conflicts and how to find present a doctor's available time slots.

I've also read through the _System Design Considerations_ and this tells me that at least I don't have to implement answers to this in this time boxed exercise. First thing that comes to mind is handling concurrent writes from multiple nodes (as we scale), as we create an appointment. Nodes 1 and 2 may see that the doctor is available at 9:00 AM and both may create an appointment at the same time, initial solution is to do db locking of timeslots for that hour.

## Disclaimer
I'll be using GitHub Copilot as it's a tool meant to be used to aid us in our work. Plus the interview with Lachlan and Mike seem to point out that AI tools are meant to be used.