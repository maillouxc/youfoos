# YouFoos

## Description

The world's most sophisticated Foosball Analytics system!

YouFoos is a powerful analyics platform that collects data directly from your
foosball table and sends it to a server for analytics and stat tracking.

Protyped as a senior software engineering project for an unnamed software company, 
the system was designed so that players sign in to their foosball table using office RFID 
keycards. Their play actions would be recorded via sensors installed on the modified table
and their analytics would be sent to a backend of servers to be crunched.
Users could sign into a powerful web application hosted on the local servers and see their
statistics in a detailed and insightful way. 

After the senior project, one of the original students took over maintaining it and continued
adding additional features. The project is currently being refined and overhauled in preparation for
the v2.0 release which will culminate with a reusable solution that can be built by anyone easily
and used in any office, bar, or even home. The exact release date that this v2.0 release will be ready by
is still unknown as my development time is not always plentiful. However, when that v2.0 release happens,
the project will be able to be used by ANYONE, and I intend on making kits available for order online.
I am also considering the financial possibility of hosting cloud-based servers, negating the need for
individuals to host YouFoos themselves.


## Installation

### Table software

It is assumed you are running the software on a Raspberry Pi 3B+ with the
Raspbian Lite OS (preferably a clean, fresh copy). Newer Pi models may 
work with the software, but we make no guarantees.

It is highly recommended that the Pi is run only in headless mode,
without the X GUI active. The easiest way to configure the Pi is via SSH.

1. Copy everything in `/table` in this repository to the Pi. You can either
clone the code from Github, which can be annonying as it requires a deploy
key for github to be setup and also copies all of the other project code,
which is unneccesary, or you can transfer it via USB stick or similar.

2. Run `install.sh` to install all dependencies and setup the software.

### Backend

1. Clone the latest code to the machine where the software will be deployed.

2. Prepare the release mode configuration files.

3. Ensure all necessary services (MongoDB, RabbitMQ, etc.) are available.

4. Using Powershell as administrator - start each microservice in release mode.

5. Start the API in release mode.

6. Start the web server to serve the web app in release mode.

6. Run the data remediation tool if necessary.

### Configuration

The 3 backend services each have several configuration files.

- `launchSettings.json` overrides all of the below and is used by visual studio during development builds.
- `appsettings.json` is always loaded and is used for common config options.
- `appsettings.Development.json` is used only in development builds.
- `appsettings.Release.json` does not get checked in to the repository - it contains production config info/secrets
- `appsettings.Release.Template.json` is the file used to specify what the real `appsettings.Release.json` should look like - you will need to create that file and fill it out manually if you wish to deploy in production mode.

### Hardware

Hardware installation guide will be written later.

## Usage

### Playing a game

1. Press the start button.

2. Scan your ID badge.

3. Claim your position by pressing lit up green button.

4. Repeat for remaining players (either 1v1 or 2v2 allowed).

5. When ready to start, press red button again and play.

6. When a goal is scored, press the green button to claim it as your goal.

7. When the game ends, press the red button to finalize the results.

If you scan your card twice when signing in, it will release you from your first spot and allow you to pick a different spot.

If a mistake is made, hold the red button to undo the goal.
