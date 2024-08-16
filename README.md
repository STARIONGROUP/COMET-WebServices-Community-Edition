<img src="https://github.com/STARIONGROUP/COMET-WebServices-Community-Edition/raw/development/COMET-Community-Edition.jpg">

## Introduction

The CDP4-COMET Webservices is the Starion Group **Concurrent Design** REST API based on ECSS-E-TM-10-25. COMET is the Starion Group Concurrent Design Solution that allows a team of engineers to perform Concurrent Design. COMET is an implementation of ECSS-E-TM-10-25A Annex A and C. ECSS-E-TM-10-25A Annex A is the so-called master model that is expressed in UML. ECSS-E-TM-10-25A Annex C describes the REST API. 

Read the [Wiki](https://github.com/STARIONGROUP/COMET-WebServices-Community-Edition/wiki) for detailed information on installation, architecture and much more.

## Concurrent Design

The Concurrent Design method is an approach to design activities in which all design disciplines and stakeholders are brought together to create an integrated design in a collaborative way of working.

The Concurrent Design method brings many advantages to the early design phase by providing a structure for this otherwise chaotic phase. Many design concepts have been implemented in the Concurrent Design method to help a team of stakeholders perform their task. The design work is done in collocated sessions with all stakeholders involved and present, creating an integrated design and enabling good communication and exchange of information between team members.

To read more about Concurrent Design and how to use the COMET IME Desktop application to perform concurrent design please read our documentation at https://www.stariongroup.eu/document/cdp4-comet-manual/

## Build Status

GitHub actions are used to build and test the library

Branch | Build Status
------- | :------------
Master | ![Build Status](https://github.com/STARIONGROUP/COMET-WebServices-Community-Edition/actions/workflows/CodeQuality.yml/badge.svg?branch=master)
Development | ![Build Status](https://github.com/STARIONGROUP/COMET-WebServices-Community-Edition/actions/workflows/CodeQuality.yml/badge.svg?branch=development)

## GitHub Statistics

 ![GitHub Downloads](https://img.shields.io/github/downloads/STARIONGROUP/COMET-WebServices-Community-Edition/total.svg)
 ![GitHub issues](https://img.shields.io/github/issues/STARIONGROUP/COMET-WebServices-Community-Edition.svg)
![GitHub issues](https://img.shields.io/github/issues-pr/STARIONGROUP/COMET-WebServices-Community-Edition.svg)
![GitHub Commit Activity](https://img.shields.io/github/commit-activity/t/STARIONGROUP/COMET-WebServices-Community-Edition.svg)
![GitHub Contrinutors](https://img.shields.io/github/contributors/STARIONGROUP/COMET-WebServices-Community-Edition.svg)
![GitHub release dates](https://img.shields.io/github/release-date/STARIONGROUP/COMET-WebServices-Community-Edition.svg)
![GitHub release](https://img.shields.io/github/release/STARIONGROUP/COMET-WebServices-Community-Edition.svg)

## SonarQube Status:

[![SonarCloud](https://sonarcloud.io/images/project_badges/sonarcloud-white.svg)](https://sonarcloud.io/summary/new_code?id=STARIONGROUP_CDP4-COMET-WebServices-Community-Edition)

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=STARIONGROUP_CDP4-COMET-WebServices-Community-Edition&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=STARIONGROUP_CDP4-COMET-WebServices-Community-Edition)
[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=STARIONGROUP_CDP4-COMET-WebServices-Community-Edition&metric=code_smells)](https://sonarcloud.io/summary/new_code?id=STARIONGROUP_CDP4-COMET-WebServices-Community-Edition)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=STARIONGROUP_CDP4-COMET-WebServices-Community-Edition&metric=bugs)](https://sonarcloud.io/summary/new_code?id=STARIONGROUP_CDP4-COMET-WebServices-Community-Edition)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=STARIONGROUP_CDP4-COMET-WebServices-Community-Edition&metric=coverage)](https://sonarcloud.io/summary/new_code?id=STARIONGROUP_CDP4-COMET-WebServices-Community-Edition)
[![Duplicated Lines (%)](https://sonarcloud.io/api/project_badges/measure?project=STARIONGROUP_CDP4-COMET-WebServices-Community-Edition&metric=duplicated_lines_density)](https://sonarcloud.io/summary/new_code?id=STARIONGROUP_CDP4-COMET-WebServices-Community-Edition)
[![Lines of Code](https://sonarcloud.io/api/project_badges/measure?project=STARIONGROUP_CDP4-COMET-WebServices-Community-Edition&metric=ncloc)](https://sonarcloud.io/summary/new_code?id=STARIONGROUP_CDP4-COMET-WebServices-Community-Edition)
[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=STARIONGROUP_CDP4-COMET-WebServices-Community-Edition&metric=sqale_rating)](https://sonarcloud.io/summary/new_code?id=STARIONGROUP_CDP4-COMET-WebServices-Community-Edition)
[![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=STARIONGROUP_CDP4-COMET-WebServices-Community-Edition&metric=reliability_rating)](https://sonarcloud.io/summary/new_code?id=STARIONGROUP_CDP4-COMET-WebServices-Community-Edition)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=STARIONGROUP_CDP4-COMET-WebServices-Community-Edition&metric=security_rating)](https://sonarcloud.io/summary/new_code?id=STARIONGROUP_CDP4-COMET-WebServices-Community-Edition)
[![Technical Debt](https://sonarcloud.io/api/project_badges/measure?project=STARIONGROUP_CDP4-COMET-WebServices-Community-Edition&metric=sqale_index)](https://sonarcloud.io/summary/new_code?id=STARIONGROUP_CDP4-COMET-WebServices-Community-Edition)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=STARIONGROUP_CDP4-COMET-WebServices-Community-Edition&metric=vulnerabilities)](https://sonarcloud.io/summary/new_code?id=STARIONGROUP_CDP4-COMET-WebServices-Community-Edition)

## COMET-SDK

The COMET Webservices make use of the [COMET-SDK](https://github.com/STARIONGROUP/COMET-SDK-Community-Edition).

## Web Framework

The COMET Webservices are built on top of the [Carter](https://github.com/CarterCommunity/Carter). Carter is a framework that is a thin layer of extension methods and functionality over ASP.NET Core allowing the code to be more explicit and most importantly more enjoyable.

## ecss-10-25-annexc-integration-tests

The ECSS-E-TM-10-25 Annex C integration tests are used to validate the correctness of the implementation. These integration tests are available on [Github](https://github.com/STARIONGROUP/ecss-10-25-annexc-integration-tests)

## License

The COMET Webservices Community Edition are provided to the community under the GNU Affero General Public License. The COMET Community Edition relies on open source and proprietary licensed components. Some of these components have a license that is not compatible with the GPL or AGPL. For these components Additional permission under GNU GPL version 3 section 7 are granted. See the license files for the details. The license can be found [here](LICENSE).

The [Starion Group](https://www.stariongroup.eu) also provides the [COMET Web Services Enterprise Edition](https://github.com/STARIONGROUP/COMET-WebServices-Community-Edition/wiki/COMET-Web-Services-Enterprise-Edition) which comes with commercial support and more features. [Contact](https://www.stariongroup.eu/contact) us for more details.

## Contributions

Contributions to the code-base are welcome. However, before we can accept your contributions we ask any contributor to sign the Contributor License Agreement (CLA) and send this digitaly signed to s.gerene@stariongroup.eu. You can find the CLA's in the CLA folder.
