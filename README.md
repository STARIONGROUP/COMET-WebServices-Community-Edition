<img src="https://github.com/RHEAGROUP/CDP4-WebServices-Community-Edition/raw/master/CDP-Community-Edition.png" width="250">

## Introduction

The Concurrent Design Platform 4 (CDP4) Webservices is the RHEA Group Concurrent Design REST API. The CDP4 is the RHEA Group Concurrent Design Solution that allows a team of engineers to perform Concurrent Design. The CDP4 is an implementation of ECSS-E-TM-10-25A Annex A and C. ECSS-E-TM-10-25A Annex A is the so-called master model that is expressed in UML. ECSS-E-TM-10-25A Annex C describes the REST API. 

Read the [Wiki](https://github.com/RHEAGROUP/CDP4-WebServices-Community-Edition/wiki) for detailed information on installation, architecture and much more.

## Concurrent Design

The Concurrent Design method is an approach to design activities in which all design disciplines and stakeholders are brought together to create an integrated design in a collaborative way of working.

The Concurrent Design method brings many advantages to the early design phase by providing a structure for this otherwise chaotic phase. Many design concepts have been implemented in the Concurrent Design method to help a team of stakeholders perform their task. The design work is done in collocated sessions with all stakeholders involved and present, creating an integrated design and enabling good communication and exchange of information between team members.

To read more about Concurrent Design and how to use the CDP4 Desktop application to perform concurrent design please read our documentation at http://cdp4docs.rheagroup.com/

## Build Status

AppVeyor is used to build and test the CDP4 Webservices

Branch | Build Status
------- | :------------
Master |  [![Build Status](https://ci.appveyor.com/api/projects/status/ojrxyxsnwtfd6med/branch/master?svg=true)](https://ci.appveyor.com/api/projects/status/ojrxyxsnwtfd6med)
Development |  [![Build Status](https://ci.appveyor.com/api/projects/status/ojrxyxsnwtfd6med/branch/development?svg=true)](https://ci.appveyor.com/api/projects/status/ojrxyxsnwtfd6med)

[![Build history](https://buildstats.info/appveyor/chart/samatrhea/cdp4-webservices-community-edition)](https://ci.appveyor.com/project/samatrhea/cdp4-webservices-community-edition/history)

## Statistics

  - Downloads: ![GitHub All Releases](https://img.shields.io/github/downloads/RHEAGROUP/CDP4-WebServices-Community-Edition/total.svg)
  - Issues:![GitHub issues](https://img.shields.io/github/issues/RHEAGROUP/CDP4-WebServices-Community-Edition.svg)

## CDP4-SDK

The Concurrent Design Platform 4 (CDP4) Webservices make use of the [CDP4-SDK](http://sdk.cdp4.org/).

## Web Framework

The CDP4 Webservices are built on top of the [Nancy](http://nancyfx.org/). Nancy is a lightweight, low-ceremony, framework for building HTTP based services on .NET and Mono. 

## ecss-10-25-annexc-integration-tests

The ECSS-E-TM-10-25 Annex C integration tests are used to validate the correctness of the implementation. These integration tests are available on [Github](https://github.com/RHEAGROUP/ecss-10-25-annexc-integration-tests)

## Docker Compose

### Introduction

This repository includes a handy script to run the webservices in a dockerized fashion. This is intended for development use. For docker compose installations of the webservices, including in production environments, refer to [CDP4-Docker-Compose](https://github.com/RHEAGROUP/CDP4-Docker-Compose) which uses the officially released images.

The script makes a release build of the source code, creates an image, pulls the latest database image and fires up the containers in a linked fashion.

### Prerequisites

MSBuild should be available through command line on your system. If you have Visual Studio installed this means you simply need to add a path to it (e.g. `C:\Program Files (x86)\Microsoft Visual Studio\2019\Professional\MSBuild\Current\Bin`) to your user Environmental Variables.

You can also have standalone msbuild installed if you dont have Visual Studio for ease of use from here: https://visualstudio.microsoft.com/thank-you-downloading-visual-studio/?sku=BuildTools&rel=16 Make sure to install `.NET Desktop Build Tools` when presented with the choice.

Add the tool installation path (e.g. `C:\Program Files (x86)\Microsoft Visual Studio\2019\BuildTools\MSBuild\Current\Bin`) to your PATH environmental variable, and restart your command line.

### Usage

To compose the containers use the provided `compose.bat` file in a cmd environment like so:

```
C:\path\to\CDP4-WebServices-Community-Edition>compose.bat [command]
```

Available commands:

- `build` - (default) builds the solution, creates the images and runs the containers.
- `strt` - starts the containers if they already have been run and stopped.
- `stp` - stops the running containers without removing them.
- `up` - runs containers without rebuilding them.
- `down` - stops and removes the containers. Volume information is not lost.
- `reboot` - performs the `down` and `up` commands in sequence.
- `rebuild` - performs the `down` and `rebuild` commands in sequence.

To verify that the services are running navigate to `localhost:5000/SiteDirectory` you should be prompted for standard credentials which are `admin/pass`.

## License

The CDP4 Webservices Community Edition are provided to the community under the GNU Affero General Public License. The CDP4 Community Edition relies on open source and proprietary licensed components. Some of these components have a license that is not compatible with the GPL or AGPL. For these components Additional permission under GNU GPL version 3 section 7 are granted. See the license files for the details. The license can be found [here](LICENSE).

The [RHEA Group](https://www.rheagroup.com) also provides the [CDP4 Web Services Enterprise Edition](https://github.com/RHEAGROUP/CDP4-WebServices-Community-Edition/wiki/CDP4-Web-Services-Enterprise-Edition) which comes with commercial support and more features. [Contact](https://www.rheagroup.com/contact) us for more details.

## Contributions

Contributions to the code-base are welcome. However, before we can accept your contributions we ask any contributor to sign the Contributor License Agreement (CLA) and send this digitaly signed to s.gerene@rheagroup.com. You can find the CLA's in the CLA folder.
