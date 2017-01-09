# Dashboard

## Nota Bene

At the current moment the main goal is to finish back-end services part, front-end client part comes later.

## Overview

This projects implements OS and hardware monitoring dashboard system for remote servers. 
It uses custom Web Socket protocol implementation with encrypted communication. The Main Service works as a demultiplexer for the recieved from the Monitoring Service data. All data is in cozy Json format.

### Arcitecture

The monitoring system has 3 main layers:

1. The Monitor package wich implements methods for OS and hardware information retrieval (Dashboard.Server.Monitoring.Monitor).
2. The Monitoring Service which broadcasts the data to the Main Service (Dashboard.Server.Monitoring.Service).
3. The Main Service which communicates with clients (Dashboard.Server.Service).

The workflow is perfectly explained in the UML Sequence Diagram below.

![Alt text](/img/seq_di.png)

### Motivation

Sometimes SA needs to check perfomance of the certain server in the enviroment and he should connect to remote server using Remote Desktop Connection (RDC), temviewer etc and go to Task Manager manually. There are many solutions to that problem, but all of them cost a lot of money. This is a free to use software with open source code which can be adjusted to any needs.

Why .Net Core? Microsoft wants .Net community to move forward, and I, beeing a great fan of the Microsoft will always support that direction. And the other reason, actually the main one - is that the Monitoring part can be replaced with another implementation for certain OS and connected to the Main Service using given protocol, but the Main Service part can be still used. So that software partly fits any OS.

## Tech stack

* .Net Core 1.1 (C#)
* .Net 4.6.1 (C#)
* AngularJS 2 (proposed)

### Tools

* Visual Studio 2017 RC
* Jetbrains dotTrace
* Jetbrains dotMemory

## Issues

* Waiting for incoming data consumes to much CPU (NetworkStream.DataAvailable propperty in a while loop).

## Future work

* Referencing .Net Core class library from .Net Standard to reduce redudency
* Client implementation
* Performance improvements
* Remote server's services and processes management
