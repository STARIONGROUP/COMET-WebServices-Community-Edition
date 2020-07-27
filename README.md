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
Master |  [![Build status](https://ci.appveyor.com/api/projects/status/7wmyvbgvdncq4sd9/branch/master?svg=true)](https://ci.appveyor.com/project/rheagroup/cdp4-webservices-community-edition/branch/master)
Development |  [![Build status](https://ci.appveyor.com/api/projects/status/7wmyvbgvdncq4sd9/branch/development?svg=true)](https://ci.appveyor.com/project/rheagroup/cdp4-webservices-community-edition/branch/development)

[![Build history](https://buildstats.info/appveyor/chart/rheagroup/cdp4-webservices-community-edition)](https://ci.appveyor.com/project/rheagroup/cdp4-webservices-community-edition/history)

## Statistics

  - Downloads: ![GitHub All Releases](https://img.shields.io/github/downloads/RHEAGROUP/CDP4-WebServices-Community-Edition/total.svg)
  - Issues:![GitHub issues](https://img.shields.io/github/issues/RHEAGROUP/CDP4-WebServices-Community-Edition.svg)

## SonarQube Status:
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=RHEAGROUP_CDP4-WebServices-Community-Edition&metric=alert_status)](https://sonarcloud.io/dashboard?id=RHEAGROUP_CDP4-WebServices-Community-Edition)
[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=RHEAGROUP_CDP4-WebServices-Community-Edition&metric=sqale_rating)](https://sonarcloud.io/dashboard?id=RHEAGROUP_CDP4-WebServices-Community-Edition)
[![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=RHEAGROUP_CDP4-WebServices-Community-Edition&metric=reliability_rating)](https://sonarcloud.io/dashboard?id=RHEAGROUP_CDP4-WebServices-Community-Edition)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=RHEAGROUP_CDP4-WebServices-Community-Edition&metric=security_rating)](https://sonarcloud.io/dashboard?id=RHEAGROUP_CDP4-WebServices-Community-Edition)

[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=RHEAGROUP_CDP4-WebServices-Community-Edition&metric=coverage)](https://sonarcloud.io/dashboard?id=RHEAGROUP_CDP4-WebServices-Community-Edition)
[![Duplicated Lines (%)](https://sonarcloud.io/api/project_badges/measure?project=RHEAGROUP_CDP4-WebServices-Community-Edition&metric=duplicated_lines_density)](https://sonarcloud.io/dashboard?id=RHEAGROUP_CDP4-WebServices-Community-Edition)

[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=RHEAGROUP_CDP4-WebServices-Community-Edition&metric=bugs)](https://sonarcloud.io/dashboard?id=RHEAGROUP_CDP4-WebServices-Community-Edition)
[![Lines of Code](https://sonarcloud.io/api/project_badges/measure?project=RHEAGROUP_CDP4-WebServices-Community-Edition&metric=ncloc)](https://sonarcloud.io/dashboard?id=RHEAGROUP_CDP4-WebServices-Community-Edition)
[![Technical Debt](https://sonarcloud.io/api/project_badges/measure?project=RHEAGROUP_CDP4-WebServices-Community-Edition&metric=sqale_index)](https://sonarcloud.io/dashboard?id=RHEAGROUP_CDP4-WebServices-Community-Edition)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=RHEAGROUP_CDP4-WebServices-Community-Edition&metric=vulnerabilities)](https://sonarcloud.io/dashboard?id=RHEAGROUP_CDP4-WebServices-Community-Edition)


## CDP4-SDK

The Concurrent Design Platform 4 (CDP4) Webservices make use of the [CDP4-SDK](http://sdk.cdp4.org/).

## Web Framework

The CDP4 Webservices are built on top of the [Nancy](http://nancyfx.org/). Nancy is a lightweight, low-ceremony, framework for building HTTP based services on .NET and Mono. 

## ecss-10-25-annexc-integration-tests

The ECSS-E-TM-10-25 Annex C integration tests are used to validate the correctness of the implementation. These integration tests are available on [Github](https://github.com/RHEAGROUP/ecss-10-25-annexc-integration-tests)

## License

The CDP4 Webservices Community Edition are provided to the community under the GNU Affero General Public License. The CDP4 Community Edition relies on open source and proprietary licensed components. Some of these components have a license that is not compatible with the GPL or AGPL. For these components Additional permission under GNU GPL version 3 section 7 are granted. See the license files for the details. The license can be found [here](LICENSE).

The [RHEA Group](https://www.rheagroup.com) also provides the [CDP4 Web Services Enterprise Edition](https://github.com/RHEAGROUP/CDP4-WebServices-Community-Edition/wiki/CDP4-Web-Services-Enterprise-Edition) which comes with commercial support and more features. [Contact](https://www.rheagroup.com/contact) us for more details.

## Contributions

Contributions to the code-base are welcome. However, before we can accept your contributions we ask any contributor to sign the Contributor License Agreement (CLA) and send this digitaly signed to s.gerene@rheagroup.com. You can find the CLA's in the CLA folder.
