using Avalonia.Controls;

using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace Keyden.Views
{
	public partial class AboutWindow : Window
	{
		public string Version => Verlite.Version.Full;
		public AboutWindow()
		{
			InitializeComponent();

			if (ActualTransparencyLevel == WindowTransparencyLevel.Mica)
			{
				Acrylic.Material.TintOpacity = 0;
				Acrylic.Material.MaterialOpacity = 0;
			}
		}

		public string MitLicense { get; } =
			"""
			Permission is hereby granted, free of charge, to any person obtaining a copy
			of this software and associated documentation files (the "Software"), to deal
			in the Software without restriction, including without limitation the rights
			to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
			copies of the Software, and to permit persons to whom the Software is
			furnished to do so, subject to the following conditions:
			
			The above copyright notice and this permission notice shall be included in all
			copies or substantial portions of the Software.
			""";

		public string Apache2License { get; } =
			"""
			Licensed under the Apache License, Version 2.0 (the "License");
			you may not use this file except in compliance with the License.
			You may obtain a copy of the License at

			http://www.apache.org/licenses/LICENSE-2.0
			""";

		public string SkiaThirdPartyLicenses { get; } =
			string.Join("\n",
				""""
				SkiaSharp and HarfBuzzSharp incorporate third party material from the projects
				listed below. The original copyright notice and the license under which
				said material was received is set forth below.

				################################################################################
				# ANGLE
				# https://github.com/Microsoft/angle
				################################################################################

				// Copyright (C) 2002-2013 The ANGLE Project Authors. 
				// Portions Copyright (C) Microsoft Corporation.
				//
				// BSD License
				//
				// All rights reserved.
				//
				// Redistribution and use in source and binary forms, with or without
				// modification, are permitted provided that the following conditions
				// are met:
				//
				//     Redistributions of source code must retain the above copyright
				//     notice, this list of conditions and the following disclaimer.
				//
				//     Redistributions in binary form must reproduce the above 
				//     copyright notice, this list of conditions and the following
				//     disclaimer in the documentation and/or other materials provided
				//     with the distribution.
				//
				//     Neither the name of TransGaming Inc., Google Inc., 3DLabs Inc.
				//     Ltd., Microsoft Corporation, nor the names of their contributors
				//     may be used to endorse or promote products derived from this
				//     software without specific prior written permission.
				//
				// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
				// "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT 
				// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
				// FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE
				// COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
				// INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING,
				// BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
				// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
				// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT
				// LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN
				// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
				// POSSIBILITY OF SUCH DAMAGE.

				################################################################################
				# END: ANGLE
				################################################################################

				################################################################################
				# HarfBuzz
				# https://github.com/harfbuzz/harfbuzz
				################################################################################

				HarfBuzz is licensed under the so-called "Old MIT" license.  Details follow.
				For parts of HarfBuzz that are licensed under different licenses see individual
				files names COPYING in subdirectories where applicable.

				Copyright © 2010,2011,2012  Google, Inc.
				Copyright © 2012  Mozilla Foundation
				Copyright © 2011  Codethink Limited
				Copyright © 2008,2010  Nokia Corporation and/or its subsidiary(-ies)
				Copyright © 2009  Keith Stribley
				Copyright © 2009  Martin Hosken and SIL International
				Copyright © 2007  Chris Wilson
				Copyright © 2006  Behdad Esfahbod
				Copyright © 2005  David Turner
				Copyright © 2004,2007,2008,2009,2010  Red Hat, Inc.
				Copyright © 1998-2004  David Turner and Werner Lemberg

				For full copyright notices consult the individual files in the package.


				Permission is hereby granted, without written agreement and without
				license or royalty fees, to use, copy, modify, and distribute this
				software and its documentation for any purpose, provided that the
				above copyright notice and the following two paragraphs appear in
				all copies of this software.

				IN NO EVENT SHALL THE COPYRIGHT HOLDER BE LIABLE TO ANY PARTY FOR
				DIRECT, INDIRECT, SPECIAL, INCIDENTAL, OR CONSEQUENTIAL DAMAGES
				ARISING OUT OF THE USE OF THIS SOFTWARE AND ITS DOCUMENTATION, EVEN
				IF THE COPYRIGHT HOLDER HAS BEEN ADVISED OF THE POSSIBILITY OF SUCH
				DAMAGE.

				THE COPYRIGHT HOLDER SPECIFICALLY DISCLAIMS ANY WARRANTIES, INCLUDING,
				BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
				FITNESS FOR A PARTICULAR PURPOSE.  THE SOFTWARE PROVIDED HEREUNDER IS
				ON AN "AS IS" BASIS, AND THE COPYRIGHT HOLDER HAS NO OBLIGATION TO
				PROVIDE MAINTENANCE, SUPPORT, UPDATES, ENHANCEMENTS, OR MODIFICATIONS.

				################################################################################
				# END: HarfBuzz
				################################################################################

				################################################################################
				# skia
				# https://github.com/google/skia
				################################################################################

				// Copyright (c) 2011 Google Inc. All rights reserved.
				//
				// Redistribution and use in source and binary forms, with or without
				// modification, are permitted provided that the following conditions are
				// met:
				//
				//    * Redistributions of source code must retain the above copyright
				// notice, this list of conditions and the following disclaimer.
				//    * Redistributions in binary form must reproduce the above
				// copyright notice, this list of conditions and the following disclaimer
				// in the documentation and/or other materials provided with the
				// distribution.
				//    * Neither the name of Google Inc. nor the names of its
				// contributors may be used to endorse or promote products derived from
				// this software without specific prior written permission.
				//
				// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
				// "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
				// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
				// A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
				// OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
				// SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
				// LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
				// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
				// THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
				// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
				// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

				--------------------------------------------------------------------------------

				################################################################################
				# END: skia
				################################################################################

				################################################################################
				# etc1
				# https://github.com/google/skia
				################################################################################
				Apache License

				Version 2.0, January 2004

				http://www.apache.org/licenses/

				TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION

				1. Definitions.

				"License" shall mean the terms and conditions for use, reproduction, and
				distribution as defined by Sections 1 through 9 of this document.

				"Licensor" shall mean the copyright owner or entity authorized by the
				copyright owner that is granting the License.

				"Legal Entity" shall mean the union of the acting entity and all other
				entities that control, are controlled by, or are under common control with
				that entity. For the purposes of this definition, "control" means (i) the
				power, direct or indirect, to cause the direction or management of such 
				entity, whether by contract or otherwise, or (ii) ownership of fifty 
				percent (50%) or more of the outstanding shares, or (iii) beneficial 
				ownership of such entity.

				"You" (or "Your") shall mean an individual or Legal Entity exercising 
				permissions granted by this License.

				"Source" form shall mean the preferred form for making modifications, 
				including but not limited to software source code, documentation 
				source, and configuration files.

				"Object" form shall mean any form resulting from mechanical transformation 
				or translation of a Source form, including but not limited to compiled 
				object code, generated documentation, and conversions to other media types.

				"Work" shall mean the work of authorship, whether in Source or Object 
				form, made available under the License, as indicated by a copyright 
				notice that is included in or attached to the work (an example is 
				provided in the Appendix below).

				"Derivative Works" shall mean any work, whether in Source or Object 
				form, that is based on (or derived from) the Work and for which the 
				editorial revisions, annotations, elaborations, or other modifications 
				represent, as a whole, an original work of authorship. For the purposes 
				of this License, Derivative Works shall not include works that remain 
				separable from, or merely link (or bind by name) to the interfaces of, 
				the Work and Derivative Works thereof.

				"Contribution" shall mean any work of authorship, including the original 
				version of the Work and any modifications or additions to that Work or 
				Derivative Works thereof, that is intentionally submitted to Licensor 
				for inclusion in the Work by the copyright owner or by an individual or 
				Legal Entity authorized to submit on behalf of the copyright owner. For 
				the purposes of this definition, "submitted" means any form of electronic, 
				verbal, or written communication sent to the Licensor or its 
				representatives, including but not limited to communication on electronic 
				mailing lists, source code control systems, and issue tracking systems that 
				are managed by, or on behalf of, the Licensor for the purpose of discussing 
				and improving the Work, but excluding communication that is conspicuously 
				marked or otherwise designated in writing by the copyright owner as "Not 
				a Contribution."

				"Contributor" shall mean Licensor and any individual or Legal Entity on 
				behalf of whom a Contribution has been received by Licensor and subsequently 
				incorporated within the Work.

				2. Grant of Copyright License. Subject to the terms and conditions of this 
				License, each Contributor hereby grants to You a perpetual, worldwide, 
				non-exclusive, no-charge, royalty-free, irrevocable copyright license to 
				reproduce, prepare Derivative Works of, publicly display, publicly perform, 
				sublicense, and distribute the Work and such Derivative Works in Source or 
				Object form.

				3. Grant of Patent License. Subject to the terms and conditions of this 
				License, each Contributor hereby grants to You a perpetual, worldwide, 
				non-exclusive, no-charge, royalty-free, irrevocable (except as stated in 
				this section) patent license to make, have made, use, offer to sell, sell, 
				import, and otherwise transfer the Work, where such license applies only to 
				those patent claims licensable by such Contributor that are necessarily 
				infringed by their Contribution(s) alone or by combination of their 
				Contribution(s) with the Work to which such Contribution(s) was submitted. 
				If You institute patent litigation against any entity (including a cross-claim
				or counterclaim in a lawsuit) alleging that the Work or a Contribution 
				incorporated within the Work constitutes direct or contributory patent 
				infringement, then any patent licenses granted to You under this License 
				for that Work shall terminate as of the date such litigation is filed.

				4. Redistribution. You may reproduce and distribute copies of the Work or 
				Derivative Works thereof in any medium, with or without modifications, and 
				in Source or Object form, provided that You meet the following conditions:

				You must give any other recipients of the Work or Derivative Works a copy of 
				this License; and
				You must cause any modified files to carry prominent notices stating that 
				You changed the files; and
				You must retain, in the Source form of any Derivative Works that You 
				distribute, all copyright, patent, trademark, and attribution notices 
				from the Source form of the Work, excluding those notices that do not 
				pertain to any part of the Derivative Works; and
				If the Work includes a "NOTICE" text file as part of its distribution, 
				then any Derivative Works that You distribute must include a readable 
				copy of the attribution notices contained within such NOTICE file, excluding
				those notices that do not pertain to any part of the Derivative Works, in
				at least one of the following places: within a NOTICE text file distributed 
				as part of the Derivative Works; within the Source form or documentation, if 
				provided along with the Derivative Works; or, within a display generated by 
				the Derivative Works, if and wherever such third-party notices normally 
				appear. The contents of the NOTICE file are for informational purposes 
				only and do not modify the License. You may add Your own attribution 
				notices within Derivative Works that You distribute, alongside or as 
				an addendum to the NOTICE text from the Work, provided that such additional 
				attribution notices cannot be construed as modifying the License. 

				You may add Your own copyright statement to Your modifications and may provide
				additional or different license terms and conditions for use, reproduction, or
				distribution of Your modifications, or for any such Derivative Works as a 
				whole, provided Your use, reproduction, and distribution of the Work otherwise 
				complies with the conditions stated in this License.
				5. Submission of Contributions. Unless You explicitly state otherwise, any 
				Contribution intentionally submitted for inclusion in the Work by You to the 
				Licensor shall be under the terms and conditions of this License, without any 
				additional terms or conditions. Notwithstanding the above, nothing herein 
				shall supersede or modify the terms of any separate license agreement you 
				may have executed with Licensor regarding such Contributions.

				6. Trademarks. This License does not grant permission to use the trade names, 
				trademarks, service marks, or product names of the Licensor, except as 
				required for reasonable and customary use in describing the origin of the 
				Work and reproducing the content of the NOTICE file.

				7. Disclaimer of Warranty. Unless required by applicable law or agreed to 
				in writing, Licensor provides the Work (and each Contributor provides its 
				Contributions) on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF 
				ANY KIND, either express or implied, including, without limitation, any 
				warranties or conditions of TITLE, NON-INFRINGEMENT, MERCHANTABILITY, or 
				FITNESS FOR A PARTICULAR PURPOSE. You are solely responsible for determining 
				the appropriateness of using or redistributing the Work and assume any risks 
				associated with Your exercise of permissions under this License.

				8. Limitation of Liability. In no event and under no legal theory, whether in
				tort (including negligence), contract, or otherwise, unless required by 
				applicable law (such as deliberate and grossly negligent acts) or agreed to 
				in writing, shall any Contributor be liable to You for damages, including 
				any direct, indirect, special, incidental, or consequential damages of any 
				character arising as a result of this License or out of the use or inability 
				to use the Work (including but not limited to damages for loss of goodwill, 
				work stoppage, computer failure or malfunction, or any and all other 
				commercial damages or losses), even if such Contributor has been advised 
				of the possibility of such damages.

				9. Accepting Warranty or Additional Liability. While redistributing the 
				Work or Derivative Works thereof, You may choose to offer, and charge a 
				fee for, acceptance of support, warranty, indemnity, or other liability 
				obligations and/or rights consistent with this License. However, in accepting
				such obligations, You may act only on Your own behalf and on Your sole 
				responsibility, not on behalf of any other Contributor, and only if You
				agree to indemnify, defend, and hold each Contributor harmless for any 
				liability incurred by, or claims asserted against, such Contributor by 
				reason of your accepting any such warranty or additional liability.

				END OF TERMS AND CONDITIONS

				################################################################################
				# END: etc1
				################################################################################

				################################################################################
				# gif
				################################################################################

				/* ***** BEGIN LICENSE BLOCK *****
				 * Version: MPL 1.1/GPL 2.0/LGPL 2.1
				 *
				 * The contents of this file are subject to the Mozilla Public License Version
				 * 1.1 (the "License"); you may not use this file except in compliance with
				 * the License. You may obtain a copy of the License at
				 * http://www.mozilla.org/MPL/
				 *
				 * Software distributed under the License is distributed on an "AS IS" basis,
				 * WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License
				 * for the specific language governing rights and limitations under the
				 * License.
				 *
				 * The Original Code is mozilla.org code.
				 *
				 * The Initial Developer of the Original Code is
				 * Netscape Communications Corporation.
				 * Portions created by the Initial Developer are Copyright (C) 1998
				 * the Initial Developer. All Rights Reserved.
				 *
				 * Contributor(s):
				 *   Chris Saari <saari@netscape.com>
				 *   Apple Computer
				 *
				 * Alternatively, the contents of this file may be used under the terms of
				 * either the GNU General Public License Version 2 or later (the "GPL"), or
				 * the GNU Lesser General Public License Version 2.1 or later (the "LGPL"),
				 * in which case the provisions of the GPL or the LGPL are applicable instead
				 * of those above. If you wish to allow use of your version of this file only
				 * under the terms of either the GPL or the LGPL, and not to allow others to
				 * use your version of this file under the terms of the MPL, indicate your
				 * decision by deleting the provisions above and replace them with the notice
				 * and other provisions required by the GPL or the LGPL. If you do not delete
				 * the provisions above, a recipient may use your version of this file under
				 * the terms of any one of the MPL, the GPL or the LGPL.
				 *
				 * ***** END LICENSE BLOCK ***** */

				################################################################################
				# END: gif
				################################################################################

				################################################################################
				# libpng
				################################################################################

				This copy of the libpng notices is provided for your convenience.  In case of
				any discrepancy between this copy and the notices in the file png.h that is
				included in the libpng distribution, the latter shall prevail.

				COPYRIGHT NOTICE, DISCLAIMER, and LICENSE:

				If you modify libpng you may insert additional notices immediately following
				this sentence.

				This code is released under the libpng license.

				libpng versions 1.0.7, July 1, 2000 through 1.6.22rc01, May 14, 2016 are
				Copyright (c) 2000-2002, 2004, 2006-2016 Glenn Randers-Pehrson, are
				derived from libpng-1.0.6, and are distributed according to the same
				disclaimer and license as libpng-1.0.6 with the following individuals
				added to the list of Contributing Authors:

				   Simon-Pierre Cadieux
				   Eric S. Raymond
				   Mans Rullgard
				   Cosmin Truta
				   Gilles Vollant
				   James Yu

				and with the following additions to the disclaimer:

				   There is no warranty against interference with your enjoyment of the
				   library or against infringement.  There is no warranty that our
				   efforts or the library will fulfill any of your particular purposes
				   or needs.  This library is provided with all faults, and the entire
				   risk of satisfactory quality, performance, accuracy, and effort is with
				   the user.

				Some files in the "contrib" directory and some configure-generated
				files that are distributed with libpng have other copyright owners and
				are released under other open source licenses.

				libpng versions 0.97, January 1998, through 1.0.6, March 20, 2000, are
				Copyright (c) 1998-2000 Glenn Randers-Pehrson, are derived from
				libpng-0.96, and are distributed according to the same disclaimer and
				license as libpng-0.96, with the following individuals added to the list
				of Contributing Authors:

				   Tom Lane
				   Glenn Randers-Pehrson
				   Willem van Schaik

				libpng versions 0.89, June 1996, through 0.96, May 1997, are
				Copyright (c) 1996-1997 Andreas Dilger, are derived from libpng-0.88,
				and are distributed according to the same disclaimer and license as
				libpng-0.88, with the following individuals added to the list of
				Contributing Authors:

				   John Bowler
				   Kevin Bracey
				   Sam Bushell
				   Magnus Holmgren
				   Greg Roelofs
				   Tom Tanner

				Some files in the "scripts" directory have other copyright owners
				but are released under this license.

				libpng versions 0.5, May 1995, through 0.88, January 1996, are
				Copyright (c) 1995-1996 Guy Eric Schalnat, Group 42, Inc.

				For the purposes of this copyright and license, "Contributing Authors"
				is defined as the following set of individuals:

				   Andreas Dilger
				   Dave Martindale
				   Guy Eric Schalnat
				   Paul Schmidt
				   Tim Wegner

				The PNG Reference Library is supplied "AS IS".  The Contributing Authors
				and Group 42, Inc. disclaim all warranties, expressed or implied,
				including, without limitation, the warranties of merchantability and of
				fitness for any purpose.  The Contributing Authors and Group 42, Inc.
				assume no liability for direct, indirect, incidental, special, exemplary,
				or consequential damages, which may result from the use of the PNG
				Reference Library, even if advised of the possibility of such damage.

				Permission is hereby granted to use, copy, modify, and distribute this
				source code, or portions hereof, for any purpose, without fee, subject
				to the following restrictions:

				  1. The origin of this source code must not be misrepresented.

				  2. Altered versions must be plainly marked as such and must not
					 be misrepresented as being the original source.

				  3. This Copyright notice may not be removed or altered from any
					 source or altered source distribution.

				The Contributing Authors and Group 42, Inc. specifically permit, without
				fee, and encourage the use of this source code as a component to
				supporting the PNG file format in commercial products.  If you use this
				source code in a product, acknowledgment is not required but would be
				appreciated.

				END OF COPYRIGHT NOTICE, DISCLAIMER, and LICENSE.

				TRADEMARK:

				The name "libpng" has not been registered by the Copyright owner
				as a trademark in any jurisdiction.  However, because libpng has
				been distributed and maintained world-wide, continually since 1995,
				the Copyright owner claims "common-law trademark protection" in any
				jurisdiction where common-law trademark is recognized.

				OSI CERTIFICATION:

				Libpng is OSI Certified Open Source Software.  OSI Certified Open Source is
				a certification mark of the Open Source Initiative. OSI has not addressed
				the additional disclaimers inserted at version 1.0.7.

				EXPORT CONTROL:

				The Copyright owner believes that the Export Control Classification
				Number (ECCN) for libpng is EAR99, which means not subject to export
				controls or International Traffic in Arms Regulations (ITAR) because
				it is open source, publicly available software, that does not contain
				any encryption software.  See the EAR, paragraphs 734.3(b)(3) and
				734.7(b).

				Glenn Randers-Pehrson
				glennrp at users.sourceforge.net
				May 14, 2016

				################################################################################
				# END: libpng
				################################################################################

				################################################################################
				# DNG SDK
				################################################################################

				This product includes DNG technology under license by Adobe Systems
				Incorporated.

				DNG SDK License Agreement
				NOTICE TO USER:
				Adobe Systems Incorporated provides the Software and Documentation for use under
				the terms of this Agreement. Any download, installation, use, reproduction,
				modification or distribution of the Software or Documentation, or any
				derivatives or portions thereof, constitutes your acceptance of this Agreement.

				As used in this Agreement, "Adobe" means Adobe Systems Incorporated. "Software"
				means the software code, in any format, including sample code and source code,
				accompanying this Agreement. "Documentation" means the documents, specifications
				and all other items accompanying this Agreement other than the Software.

				1. LICENSE GRANT
				Software License.  Subject to the restrictions below and other terms of this
				Agreement, Adobe hereby grants you a non-exclusive, worldwide, royalty free
				license to use, reproduce, prepare derivative works from, publicly display,
				publicly perform, distribute and sublicense the Software for any purpose.

				Document License.  Subject to the terms of this Agreement, Adobe hereby grants
				you a non-exclusive, worldwide, royalty free license to make a limited number of
				copies of the Documentation for your development purposes and to publicly
				display, publicly perform and distribute such copies.  You may not modify the
				Documentation.

				2. RESTRICTIONS AND OWNERSHIP
				You will not remove any copyright or other notice included in the Software or
				Documentation and you will include such notices in any copies of the Software
				that you distribute in human-readable format.

				You will not copy, use, display, modify or distribute the Software or
				Documentation in any manner not permitted by this Agreement. No title to the
				intellectual property in the Software or Documentation is transferred to you
				under the terms of this Agreement. You do not acquire any rights to the Software
				or the Documentation except as expressly set forth in this Agreement. All rights
				not granted are reserved by Adobe.

				3. DISCLAIMER OF WARRANTY
				ADOBE PROVIDES THE SOFTWARE AND DOCUMENTATION ONLY ON AN "AS IS" BASIS WITHOUT
				WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING
				WITHOUT LIMITATION ANY WARRANTIES OR CONDITIONS OF TITLE, NON-INFRINGEMENT,
				MERCHANTABILITY OR FITNESS FOR A PARTICULAR PURPOSE. ADOBE MAKES NO WARRANTY
				THAT THE SOFTWARE OR DOCUMENTATION WILL BE ERROR-FREE. To the extent
				permissible, any warranties that are not and cannot be excluded by the foregoing
				are limited to ninety (90) days.

				4. LIMITATION OF LIABILITY
				ADOBE AND ITS SUPPLIERS SHALL NOT BE LIABLE FOR LOSS OR DAMAGE ARISING OUT OF
				THIS AGREEMENT OR FROM THE USE OF THE SOFTWARE OR DOCUMENTATION. IN NO EVENT
				WILL ADOBE BE LIABLE TO YOU OR ANY THIRD PARTY FOR ANY DIRECT, INDIRECT,
				CONSEQUENTIAL, INCIDENTAL, OR SPECIAL DAMAGES INCLUDING LOST PROFITS, LOST
				SAVINGS, COSTS, FEES, OR EXPENSES OF ANY KIND ARISING OUT OF ANY PROVISION OF
				THIS AGREEMENT OR THE USE OR THE INABILITY TO USE THE SOFTWARE OR DOCUMENTATION,
				HOWEVER CAUSED AND UNDER ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT
				LIABILITY OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE), EVEN IF ADVISED OF THE
				POSSIBILITY OF SUCH DAMAGES. ADOBE'S AGGREGATE LIABILITY AND THAT OF ITS
				SUPPLIERS UNDER OR IN CONNECTION WITH THIS AGREEMENT SHALL BE LIMITED TO THE
				AMOUNT PAID BY YOU FOR THE SOFTWARE AND DOCUMENTATION.

				5. INDEMNIFICATION
				If you choose to distribute the Software in a commercial product, you do so with
				the understanding that you agree to defend, indemnify and hold harmless Adobe
				against any losses, damages and costs arising from the claims, lawsuits or other
				legal actions arising out of such distribution.

				6. TRADEMARK USAGE
				Adobe and the DNG logo are the trademarks or registered trademarks of Adobe
				Systems Incorporated in the United States and other countries. Such trademarks
				may not be used to endorse or promote any product unless expressly permitted
				under separate agreement with Adobe. For information on how to license the DNG
				logo please go to www.adobe.com.

				7. TERM
				Your rights under this Agreement shall terminate if you fail to comply with any
				of the material terms or conditions of this Agreement. If all your rights under
				this Agreement terminate, you will immediately cease use and distribution of the
				Software and Documentation.

				8. GOVERNING LAW AND JURISDICTION. This Agreement is governed by the statutes
				and laws of the State of California, without regard to the conflicts of law
				principles thereof. The federal and state courts located in Santa Clara County,
				California, USA, will have non-exclusive jurisdiction over any dispute arising
				out of this Agreement.

				9. GENERAL
				This Agreement supersedes any prior agreement, oral or written, between Adobe
				and you with respect to the licensing to you of the Software and Documentation.
				No variation of the terms of this Agreement will be enforceable against Adobe
				unless Adobe gives its express consent in writing signed by an authorized
				signatory of Adobe. If any part of this Agreement is found void and
				unenforceable, it will not affect the validity of the balance of the Agreement,
				which shall remain valid and enforceable according to its terms.

				################################################################################
				# END: DNG SDK
				################################################################################

				################################################################################
				# expat
				################################################################################

				Copyright (c) 1998, 1999, 2000 Thai Open Source Software Center Ltd
											   and Clark Cooper
				Copyright (c) 2001, 2002, 2003, 2004, 2005, 2006 Expat maintainers.

				Permission is hereby granted, free of charge, to any person obtaining
				a copy of this software and associated documentation files (the
				"Software"), to deal in the Software without restriction, including
				without limitation the rights to use, copy, modify, merge, publish,
				distribute, sublicense, and/or sell copies of the Software, and to
				permit persons to whom the Software is furnished to do so, subject to
				the following conditions:

				The above copyright notice and this permission notice shall be included
				in all copies or substantial portions of the Software.

				THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
				EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
				MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
				IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
				CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
				TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
				SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

				################################################################################
				# END: expat
				################################################################################

				################################################################################
				# freetype
				################################################################################

				--- LICENSE.TXT ---

				The  FreeType 2  font  engine is  copyrighted  work and  cannot be  used
				legally  without a  software license.   In  order to  make this  project
				usable  to a vast  majority of  developers, we  distribute it  under two
				mutually exclusive open-source licenses.

				This means  that *you* must choose  *one* of the  two licenses described
				below, then obey  all its terms and conditions when  using FreeType 2 in
				any of your projects or products.

				  - The FreeType License, found in  the file `FTL.TXT', which is similar
					to the original BSD license *with* an advertising clause that forces
					you  to  explicitly cite  the  FreeType  project  in your  product's
					documentation.  All  details are in the license  file.  This license
					is  suited  to products  which  don't  use  the GNU  General  Public
					License.

					Note that  this license  is  compatible  to the  GNU General  Public
					License version 3, but not version 2.

				  - The GNU General Public License version 2, found in  `GPLv2.TXT' (any
					later version can be used  also), for programs which already use the
					GPL.  Note  that the  FTL is  incompatible  with  GPLv2 due  to  its
					advertisement clause.

				The contributed BDF and PCF drivers  come with a license similar to that
				of the X Window System.  It is compatible to the above two licenses (see
				file src/bdf/README and  src/pcf/README).  The same holds  for the files
				`fthash.c' and  `fthash.h'; their  code was  part of  the BDF  driver in
				earlier FreeType versions.

				The gzip module uses the zlib license (see src/gzip/zlib.h) which too is
				compatible to the above two licenses.

				The MD5 checksum support (only used for debugging in development builds)
				is in the public domain.


				--- end of LICENSE.TXT ---

				--- FTL.TXT ---

									The FreeType Project LICENSE
									----------------------------

											2006-Jan-27

									Copyright 1996-2002, 2006 by
						  David Turner, Robert Wilhelm, and Werner Lemberg



				Introduction
				============

				  The FreeType  Project is distributed in  several archive packages;
				  some of them may contain, in addition to the FreeType font engine,
				  various tools and  contributions which rely on, or  relate to, the
				  FreeType Project.

				  This  license applies  to all  files found  in such  packages, and
				  which do not  fall under their own explicit  license.  The license
				  affects  thus  the  FreeType   font  engine,  the  test  programs,
				  documentation and makefiles, at the very least.

				  This  license   was  inspired  by  the  BSD,   Artistic,  and  IJG
				  (Independent JPEG  Group) licenses, which  all encourage inclusion
				  and  use of  free  software in  commercial  and freeware  products
				  alike.  As a consequence, its main points are that:

					o We don't promise that this software works. However, we will be
					  interested in any kind of bug reports. (`as is' distribution)

					o You can  use this software for whatever you  want, in parts or
					  full form, without having to pay us. (`royalty-free' usage)

					o You may not pretend that  you wrote this software.  If you use
					  it, or  only parts of it,  in a program,  you must acknowledge
					  somewhere  in  your  documentation  that  you  have  used  the
					  FreeType code. (`credits')

				  We  specifically  permit  and  encourage  the  inclusion  of  this
				  software, with  or without modifications,  in commercial products.
				  We  disclaim  all warranties  covering  The  FreeType Project  and
				  assume no liability related to The FreeType Project.


				  Finally,  many  people  asked  us  for  a  preferred  form  for  a
				  credit/disclaimer to use in compliance with this license.  We thus
				  encourage you to use the following text:

				   """
					Portions of this software are copyright © <year> The FreeType
					Project (www.freetype.org).  All rights reserved.
				   """

				  Please replace <year> with the value from the FreeType version you
				  actually use.


				Legal Terms
				===========

				0. Definitions
				--------------

				  Throughout this license,  the terms `package', `FreeType Project',
				  and  `FreeType  archive' refer  to  the  set  of files  originally
				  distributed  by the  authors  (David Turner,  Robert Wilhelm,  and
				  Werner Lemberg) as the `FreeType Project', be they named as alpha,
				  beta or final release.

				  `You' refers to  the licensee, or person using  the project, where
				  `using' is a generic term including compiling the project's source
				  code as  well as linking it  to form a  `program' or `executable'.
				  This  program is  referred to  as  `a program  using the  FreeType
				  engine'.

				  This  license applies  to all  files distributed  in  the original
				  FreeType  Project,   including  all  source   code,  binaries  and
				  documentation,  unless  otherwise  stated   in  the  file  in  its
				  original, unmodified form as  distributed in the original archive.
				  If you are  unsure whether or not a particular  file is covered by
				  this license, you must contact us to verify this.

				  The FreeType  Project is copyright (C) 1996-2000  by David Turner,
				  Robert Wilhelm, and Werner Lemberg.  All rights reserved except as
				  specified below.

				1. No Warranty
				--------------

				  THE FREETYPE PROJECT  IS PROVIDED `AS IS' WITHOUT  WARRANTY OF ANY
				  KIND, EITHER  EXPRESS OR IMPLIED,  INCLUDING, BUT NOT  LIMITED TO,
				  WARRANTIES  OF  MERCHANTABILITY   AND  FITNESS  FOR  A  PARTICULAR
				  PURPOSE.  IN NO EVENT WILL ANY OF THE AUTHORS OR COPYRIGHT HOLDERS
				  BE LIABLE  FOR ANY DAMAGES CAUSED  BY THE USE OR  THE INABILITY TO
				  USE, OF THE FREETYPE PROJECT.

				2. Redistribution
				-----------------

				  This  license  grants  a  worldwide, royalty-free,  perpetual  and
				  irrevocable right  and license to use,  execute, perform, compile,
				  display,  copy,   create  derivative  works   of,  distribute  and
				  sublicense the  FreeType Project (in  both source and  object code
				  forms)  and  derivative works  thereof  for  any  purpose; and  to
				  authorize others  to exercise  some or all  of the  rights granted
				  herein, subject to the following conditions:

					o Redistribution of  source code  must retain this  license file
					  (`FTL.TXT') unaltered; any  additions, deletions or changes to
					  the original  files must be clearly  indicated in accompanying
					  documentation.   The  copyright   notices  of  the  unaltered,
					  original  files must  be  preserved in  all  copies of  source
					  files.

					o Redistribution in binary form must provide a  disclaimer  that
					  states  that  the software is based in part of the work of the
					  FreeType Team,  in  the  distribution  documentation.  We also
					  encourage you to put an URL to the FreeType web page  in  your
					  documentation, though this isn't mandatory.

				  These conditions  apply to any  software derived from or  based on
				  the FreeType Project,  not just the unmodified files.   If you use
				  our work, you  must acknowledge us.  However, no  fee need be paid
				  to us.

				3. Advertising
				--------------

				  Neither the  FreeType authors and  contributors nor you  shall use
				  the name of the  other for commercial, advertising, or promotional
				  purposes without specific prior written permission.

				  We suggest,  but do not require, that  you use one or  more of the
				  following phrases to refer  to this software in your documentation
				  or advertising  materials: `FreeType Project',  `FreeType Engine',
				  `FreeType library', or `FreeType Distribution'.

				  As  you have  not signed  this license,  you are  not  required to
				  accept  it.   However,  as  the FreeType  Project  is  copyrighted
				  material, only  this license, or  another one contracted  with the
				  authors, grants you  the right to use, distribute,  and modify it.
				  Therefore,  by  using,  distributing,  or modifying  the  FreeType
				  Project, you indicate that you understand and accept all the terms
				  of this license.

				4. Contacts
				-----------

				  There are two mailing lists related to FreeType:

					o freetype@nongnu.org

					  Discusses general use and applications of FreeType, as well as
					  future and  wanted additions to the  library and distribution.
					  If  you are looking  for support,  start in  this list  if you
					  haven't found anything to help you in the documentation.

					o freetype-devel@nongnu.org

					  Discusses bugs,  as well  as engine internals,  design issues,
					  specific licenses, porting, etc.

				  Our home page can be found at

					http://www.freetype.org


				--- end of FTL.TXT ---

				################################################################################
				# END: freetype
				################################################################################

				################################################################################
				# ICU
				################################################################################

				ICU License - ICU 1.8.1 and later

				   COPYRIGHT AND PERMISSION NOTICE

				   Copyright (c) 1995-2015 International Business Machines Corporation and
				   others

				   All rights reserved.

				   Permission is hereby granted, free of charge, to any person obtaining a
				   copy of this software and associated documentation files (the
				   "Software"), to deal in the Software without restriction, including
				   without limitation the rights to use, copy, modify, merge, publish,
				   distribute, and/or sell copies of the Software, and to permit persons to
				   whom the Software is furnished to do so, provided that the above
				   copyright notice(s) and this permission notice appear in all copies of
				   the Software and that both the above copyright notice(s) and this
				   permission notice appear in supporting documentation.

				   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
				   OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
				   MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT OF
				   THIRD PARTY RIGHTS. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR HOLDERS
				   INCLUDED IN THIS NOTICE BE LIABLE FOR ANY CLAIM, OR ANY SPECIAL INDIRECT
				   OR CONSEQUENTIAL DAMAGES, OR ANY DAMAGES WHATSOEVER RESULTING FROM LOSS
				   OF USE, DATA OR PROFITS, WHETHER IN AN ACTION OF CONTRACT, NEGLIGENCE OR
				   OTHER TORTIOUS ACTION, ARISING OUT OF OR IN CONNECTION WITH THE USE OR
				   PERFORMANCE OF THIS SOFTWARE.

				   Except as contained in this notice, the name of a copyright holder shall
				   not be used in advertising or otherwise to promote the sale, use or
				   other dealings in this Software without prior written authorization of
				   the copyright holder.
					 ___________________________________________________________________

				   All trademarks and registered trademarks mentioned herein are the
				   property of their respective owners.
					 ___________________________________________________________________

				Third-Party Software Licenses

				   This section contains third-party software notices and/or additional
				   terms for licensed third-party software components included within ICU
				   libraries.

				  1. Unicode Data Files and Software

				COPYRIGHT AND PERMISSION NOTICE

				Copyright © 1991-2015 Unicode, Inc. All rights reserved.
				Distributed under the Terms of Use in
				http://www.unicode.org/copyright.html.

				Permission is hereby granted, free of charge, to any person obtaining
				a copy of the Unicode data files and any associated documentation
				(the "Data Files") or Unicode software and any associated documentation
				(the "Software") to deal in the Data Files or Software
				without restriction, including without limitation the rights to use,
				copy, modify, merge, publish, distribute, and/or sell copies of
				the Data Files or Software, and to permit persons to whom the Data Files
				or Software are furnished to do so, provided that
				(a) this copyright and permission notice appear with all copies
				of the Data Files or Software,
				(b) this copyright and permission notice appear in associated
				documentation, and
				(c) there is clear notice in each modified Data File or in the Software
				as well as in the documentation associated with the Data File(s) or
				Software that the data or software has been modified.

				THE DATA FILES AND SOFTWARE ARE PROVIDED "AS IS", WITHOUT WARRANTY OF
				ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
				WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
				NONINFRINGEMENT OF THIRD PARTY RIGHTS.
				IN NO EVENT SHALL THE COPYRIGHT HOLDER OR HOLDERS INCLUDED IN THIS
				NOTICE BE LIABLE FOR ANY CLAIM, OR ANY SPECIAL INDIRECT OR CONSEQUENTIAL
				DAMAGES, OR ANY DAMAGES WHATSOEVER RESULTING FROM LOSS OF USE,
				DATA OR PROFITS, WHETHER IN AN ACTION OF CONTRACT, NEGLIGENCE OR OTHER
				TORTIOUS ACTION, ARISING OUT OF OR IN CONNECTION WITH THE USE OR
				PERFORMANCE OF THE DATA FILES OR SOFTWARE.

				Except as contained in this notice, the name of a copyright holder
				shall not be used in advertising or otherwise to promote the sale,
				use or other dealings in these Data Files or Software without prior
				written authorization of the copyright holder.

				  2. Chinese/Japanese Word Break Dictionary Data (cjdict.txt)

				 #    The Google Chrome software developed by Google is licensed under
				 #    the BSD license. Other software included in this distribution is
				 #    provided under other licenses, as set forth below.
				 #
				 #      The BSD License
				 #      http://opensource.org/licenses/bsd-license.php
				 #      Copyright (C) 2006-2008, Google Inc.
				 #
				 #      All rights reserved.
				 #
				 #      Redistribution and use in source and binary forms, with or
				 #      without modification, are permitted provided that the
				 #      following conditions are met:
				 #
				 #      Redistributions of source code must retain the above copyright
				 #        notice, this list of conditions and the following disclaimer.
				 #      Redistributions in binary form must reproduce the above
				 #        copyright notice, this list of conditions and the following
				 #        disclaimer in the documentation and/or other materials
				 #        provided with the distribution.
				 #      Neither the name of  Google Inc. nor the names of its
				 #        contributors may be used to endorse or promote products
				 #        derived from this software without specific prior written
				 #        permission.
				 #
				 #
				 #      THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND
				 #      CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES,
				 #      INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF
				 #      MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
				 #      DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
				 #      CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
				 #      SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
				 #      NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
				 #      LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
				 #      HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
				 #      CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR
				 #      OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE,
				 #      EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
				 #
				 #
				 #      The word list in cjdict.txt are generated by combining three
				 #      word lists listed below with further processing for compound
				 #      word breaking. The frequency is generated with an iterative
				 #      training against Google web corpora.
				 #
				 #      * Libtabe (Chinese)
				 #        - https://sourceforge.net/project/?group_id=1519
				 #        - Its license terms and conditions are shown below.
				 #
				 #      * IPADIC (Japanese)
				 #        - http://chasen.aist-nara.ac.jp/chasen/distribution.html
				 #        - Its license terms and conditions are shown below.
				 #
				 #      ---------COPYING.libtabe ---- BEGIN--------------------
				 #
				 #      /*
				 #       * Copyrighy (c) 1999 TaBE Project.
				 #       * Copyright (c) 1999 Pai-Hsiang Hsiao.
				 #       * All rights reserved.
				 #       *
				 #       * Redistribution and use in source and binary forms, with or without
				 #       * modification, are permitted provided that the following conditions
				 #       * are met:
				 #       *
				 #       * . Redistributions of source code must retain the above copyright
				 #       *   notice, this list of conditions and the following disclaimer.
				 #       * . Redistributions in binary form must reproduce the above copyright
				 #       *   notice, this list of conditions and the following disclaimer in
				 #       *   the documentation and/or other materials provided with the
				 #       *   distribution.
				 #       * . Neither the name of the TaBE Project nor the names of its
				 #       *   contributors may be used to endorse or promote products derived
				 #       *   from this software without specific prior written permission.
				 #       *
				 #       * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
				 #       * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
				 #       * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
				 #       * FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE
				 #       * REGENTS OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
				 #       * INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
				 #       * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
				 #       * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
				 #       * HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
				 #       * STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
				 #       * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED
				 #       * OF THE POSSIBILITY OF SUCH DAMAGE.
				 #       */
				 #
				 #      /*
				 #       * Copyright (c) 1999 Computer Systems and Communication Lab,
				 #       *                    Institute of Information Science, Academia Sinica.
				 #       * All rights reserved.
				 #       *
				 #       * Redistribution and use in source and binary forms, with or without
				 #       * modification, are permitted provided that the following conditions
				 #       * are met:
				 #       *
				 #       * . Redistributions of source code must retain the above copyright
				 #       *   notice, this list of conditions and the following disclaimer.
				 #       * . Redistributions in binary form must reproduce the above copyright
				 #       *   notice, this list of conditions and the following disclaimer in
				 #       *   the documentation and/or other materials provided with the
				 #       *   distribution.
				 #       * . Neither the name of the Computer Systems and Communication Lab
				 #       *   nor the names of its contributors may be used to endorse or
				 #       *   promote products derived from this software without specific
				 #       *   prior written permission.
				 #       *
				 #       * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
				 #       * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
				 #       * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
				 #       * FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE
				 #       * REGENTS OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
				 #       * INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
				 #       * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
				 #       * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
				 #       * HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
				 #       * STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
				 #       * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED
				 #       * OF THE POSSIBILITY OF SUCH DAMAGE.
				 #       */
				 #
				 #      Copyright 1996 Chih-Hao Tsai @ Beckman Institute, University of Illinois
				 #      c-tsai4@uiuc.edu  http://casper.beckman.uiuc.edu/~c-tsai4
				 #
				 #      ---------------COPYING.libtabe-----END------------------------------------
				 #
				 #
				 #      ---------------COPYING.ipadic-----BEGIN------------------------------------
				 #
				 #      Copyright 2000, 2001, 2002, 2003 Nara Institute of Science
				 #      and Technology.  All Rights Reserved.
				 #
				 #      Use, reproduction, and distribution of this software is permitted.
				 #      Any copy of this software, whether in its original form or modified,
				 #      must include both the above copyright notice and the following
				 #      paragraphs.
				 #
				 #      Nara Institute of Science and Technology (NAIST),
				 #      the copyright holders, disclaims all warranties with regard to this
				 #      software, including all implied warranties of merchantability and
				 #      fitness, in no event shall NAIST be liable for
				 #      any special, indirect or consequential damages or any damages
				 #      whatsoever resulting from loss of use, data or profits, whether in an
				 #      action of contract, negligence or other tortuous action, arising out
				 #      of or in connection with the use or performance of this software.
				 #
				 #      A large portion of the dictionary entries
				 #      originate from ICOT Free Software.  The following conditions for ICOT
				 #      Free Software applies to the current dictionary as well.
				 #
				 #      Each User may also freely distribute the Program, whether in its
				 #      original form or modified, to any third party or parties, PROVIDED
				 #      that the provisions of Section 3 ("NO WARRANTY") will ALWAYS appear
				 #      on, or be attached to, the Program, which is distributed substantially
				 #      in the same form as set out herein and that such intended
				 #      distribution, if actually made, will neither violate or otherwise
				 #      contravene any of the laws and regulations of the countries having
				 #      jurisdiction over the User or the intended distribution itself.
				 #
				 #      NO WARRANTY
				 #
				 #      The program was produced on an experimental basis in the course of the
				 #      research and development conducted during the project and is provided
				 #      to users as so produced on an experimental basis.  Accordingly, the
				 #      program is provided without any warranty whatsoever, whether express,
				 #      implied, statutory or otherwise.  The term "warranty" used herein
				 #      includes, but is not limited to, any warranty of the quality,
				 #      performance, merchantability and fitness for a particular purpose of
				 #      the program and the nonexistence of any infringement or violation of
				 #      any right of any third party.
				 #
				 #      Each user of the program will agree and understand, and be deemed to
				 #      have agreed and understood, that there is no warranty whatsoever for
				 #      the program and, accordingly, the entire risk arising from or
				 #      otherwise connected with the program is assumed by the user.
				 #
				 #      Therefore, neither ICOT, the copyright holder, or any other
				 #      organization that participated in or was otherwise related to the
				 #      development of the program and their respective officials, directors,
				 #      officers and other employees shall be held liable for any and all
				 #      damages, including, without limitation, general, special, incidental
				 #      and consequential damages, arising out of or otherwise in connection
				 #      with the use or inability to use the program or any product, material
				 #      or result produced or otherwise obtained by using the program,
				 #      regardless of whether they have been advised of, or otherwise had
				 #      knowledge of, the possibility of such damages at any time during the
				 #      project or thereafter.  Each user will be deemed to have agreed to the
				 #      foregoing by his or her commencement of use of the program.  The term
				 #      "use" as used herein includes, but is not limited to, the use,
				 #      modification, copying and distribution of the program and the
				 #      production of secondary products from the program.
				 #
				 #      In the case where the program, whether in its original form or
				 #      modified, was distributed or delivered to or received by a user from
				 #      any person, organization or entity other than ICOT, unless it makes or
				 #      grants independently of ICOT any specific warranty to the user in
				 #      writing, such person, organization or entity, will also be exempted
				 #      from and not be held liable to the user for any such damages as noted
				 #      above as far as the program is concerned.
				 #
				 #      ---------------COPYING.ipadic-----END------------------------------------

				  3. Lao Word Break Dictionary Data (laodict.txt)

				 #      Copyright (c) 2013 International Business Machines Corporation
				 #      and others. All Rights Reserved.
				 #
				 #      Project:    http://code.google.com/p/lao-dictionary/
				 #      Dictionary: http://lao-dictionary.googlecode.com/git/Lao-Dictionary.txt
				 #      License:    http://lao-dictionary.googlecode.com/git/Lao-Dictionary-LICENSE.txt
				 #                  (copied below)
				 #
				 #      This file is derived from the above dictionary, with slight modifications.
				 #      --------------------------------------------------------------------------------
				 #      Copyright (C) 2013 Brian Eugene Wilson, Robert Martin Campbell.
				 #      All rights reserved.
				 #
				 #      Redistribution and use in source and binary forms, with or without modification,
				 #      are permitted provided that the following conditions are met:
				 #
				 #              Redistributions of source code must retain the above copyright notice, this
				 #              list of conditions and the following disclaimer. Redistributions in binary
				 #              form must reproduce the above copyright notice, this list of conditions and
				 #              the following disclaimer in the documentation and/or other materials
				 #              provided with the distribution.
				 #
				 #      THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
				 #      ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
				 #      WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
				 #      DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR
				 #      ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
				 #      (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
				 #      LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
				 #      ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
				 #      (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
				 #      SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
				 #      --------------------------------------------------------------------------------

				  4. Burmese Word Break Dictionary Data (burmesedict.txt)

				 #      Copyright (c) 2014 International Business Machines Corporation
				 #      and others. All Rights Reserved.
				 #
				 #      This list is part of a project hosted at:
				 #        github.com/kanyawtech/myanmar-karen-word-lists
				 #
				 #      --------------------------------------------------------------------------------
				 #      Copyright (c) 2013, LeRoy Benjamin Sharon
				 #      All rights reserved.
				 #
				 #      Redistribution and use in source and binary forms, with or without modification,
				 #      are permitted provided that the following conditions are met:
				 #
				 #        Redistributions of source code must retain the above copyright notice, this
				 #        list of conditions and the following disclaimer.
				 #
				 #        Redistributions in binary form must reproduce the above copyright notice, this
				 #        list of conditions and the following disclaimer in the documentation an d/or
				 #        other materials provided with the distribution.
				 #
				 #        Neither the name Myanmar Karen Word Lists, nor the names of its
				 #        contributors may be used to endorse or promote products derived from
				 #        this software without specific prior written permission.
				 #
				 #      THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
				 #      ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
				 #      WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
				 #      DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR
				 #      ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
				 #      (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
				 #      LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
				 #      ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
				 #      (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
				 #      SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
				 #      --------------------------------------------------------------------------------

				  5. Time Zone Database

				   ICU uses the public domain data and code derived from Time Zone Database
				   for its time zone support. The ownership of the TZ database is explained
				   in BCP 175: Procedure for Maintaining the Time Zone Database section 7.

				7.  Database Ownership

				   The TZ database itself is not an IETF Contribution or an IETF
				   document.  Rather it is a pre-existing and regularly updated work
				   that is in the public domain, and is intended to remain in the public
				   domain.  Therefore, BCPs 78 [RFC5378] and 79 [RFC3979] do not apply
				   to the TZ Database or contributions that individuals make to it.
				   Should any claims be made and substantiated against the TZ Database,
				   the organization that is providing the IANA Considerations defined in
				   this RFC, under the memorandum of understanding with the IETF,
				   currently ICANN, may act in accordance with all competent court
				   orders.  No ownership claims will be made by ICANN or the IETF Trust
				   on the database or the code.  Any person making a contribution to the
				   database or code waives all rights to future claims in that
				   contribution or in the TZ Database.

				################################################################################
				# END: ICU
				################################################################################

				################################################################################
				# imgui
				################################################################################

				The MIT License (MIT)

				Copyright (c) 2014-2015 Omar Cornut and ImGui contributors

				Permission is hereby granted, free of charge, to any person obtaining a copy
				of this software and associated documentation files (the "Software"), to deal
				in the Software without restriction, including without limitation the rights
				to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
				copies of the Software, and to permit persons to whom the Software is
				furnished to do so, subject to the following conditions:

				The above copyright notice and this permission notice shall be included in all
				copies or substantial portions of the Software.

				THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
				IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
				FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
				AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
				LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
				OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
				SOFTWARE.

				################################################################################
				# END: imgui
				################################################################################

				################################################################################
				# jsoncpp
				################################################################################

				The JsonCpp library's source code, including accompanying documentation, 
				tests and demonstration applications, are licensed under the following
				conditions...

				The author (Baptiste Lepilleur) explicitly disclaims copyright in all 
				jurisdictions which recognize such a disclaimer. In such jurisdictions, 
				this software is released into the Public Domain.

				In jurisdictions which do not recognize Public Domain property (e.g. Germany as of
				2010), this software is Copyright (c) 2007-2010 by Baptiste Lepilleur, and is
				released under the terms of the MIT License (see below).

				In jurisdictions which recognize Public Domain property, the user of this 
				software may choose to accept it either as 1) Public Domain, 2) under the 
				conditions of the MIT License (see below), or 3) under the terms of dual 
				Public Domain/MIT License conditions described here, as they choose.

				The MIT License is about as close to Public Domain as a license can get, and is
				described in clear, concise terms at:

				   http://en.wikipedia.org/wiki/MIT_License

				The full text of the MIT License follows:

				========================================================================
				Copyright (c) 2007-2010 Baptiste Lepilleur

				Permission is hereby granted, free of charge, to any person
				obtaining a copy of this software and associated documentation
				files (the "Software"), to deal in the Software without
				restriction, including without limitation the rights to use, copy,
				modify, merge, publish, distribute, sublicense, and/or sell copies
				of the Software, and to permit persons to whom the Software is
				furnished to do so, subject to the following conditions:

				The above copyright notice and this permission notice shall be
				included in all copies or substantial portions of the Software.

				THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
				EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
				MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
				NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS
				BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN
				ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
				CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
				SOFTWARE.
				========================================================================
				(END LICENSE TEXT)

				The MIT license is compatible with both the GPL and commercial
				software, affording one all of the rights of Public Domain with the
				minor nuisance of being required to keep the above copyright notice
				and license text in the source code. Note also that by accepting the
				Public Domain "license" you can re-license your copy using whatever
				license you like.

				################################################################################
				# END: jsoncpp
				################################################################################

				################################################################################
				# libjpeg-turbo
				################################################################################

				libjpeg-turbo Licenses
				======================

				libjpeg-turbo is covered by three compatible BSD-style open source licenses:

				- The IJG (Independent JPEG Group) License, which is listed in
				  [README.ijg](README.ijg)

				  This license applies to the libjpeg API library and associated programs
				  (any code inherited from libjpeg, and any modifications to that code.)

				- The Modified (3-clause) BSD License, which is listed in
				  [turbojpeg.c](turbojpeg.c)

				  This license covers the TurboJPEG API library and associated programs.

				- The zlib License, which is listed in [simd/jsimdext.inc](simd/jsimdext.inc)

				  This license is a subset of the other two, and it covers the libjpeg-turbo
				  SIMD extensions.


				Complying with the libjpeg-turbo Licenses
				=========================================

				This section provides a roll-up of the libjpeg-turbo licensing terms, to the
				best of our understanding.

				1.  If you are distributing a modified version of the libjpeg-turbo source,
					then:

					1.  You cannot alter or remove any existing copyright or license notices
						from the source.

						**Origin**
						- Clause 1 of the IJG License
						- Clause 1 of the Modified BSD License
						- Clauses 1 and 3 of the zlib License

					2.  You must add your own copyright notice to the header of each source
						file you modified, so others can tell that you modified that file (if
						there is not an existing copyright header in that file, then you can
						simply add a notice stating that you modified the file.)

						**Origin**
						- Clause 1 of the IJG License
						- Clause 2 of the zlib License

					3.  You must include the IJG README file, and you must not alter any of the
						copyright or license text in that file.

						**Origin**
						- Clause 1 of the IJG License

				2.  If you are distributing only libjpeg-turbo binaries without the source, or
					if you are distributing an application that statically links with
					libjpeg-turbo, then:

					1.  Your product documentation must include a message stating:

						This software is based in part on the work of the Independent JPEG
						Group.

						**Origin**
						- Clause 2 of the IJG license

					2.  If your binary distribution includes or uses the TurboJPEG API, then
						your product documentation must include the text of the Modified BSD
						License.

						**Origin**
						- Clause 2 of the Modified BSD License

				3.  You cannot use the name of the IJG or The libjpeg-turbo Project or the
					contributors thereof in advertising, publicity, etc.

					**Origin**
					- IJG License
					- Clause 3 of the Modified BSD License

				4.  The IJG and The libjpeg-turbo Project do not warrant libjpeg-turbo to be
					free of defects, nor do we accept any liability for undesirable
					consequences resulting from your use of the software.

					**Origin**
					- IJG License
					- Modified BSD License
					- zlib License

				################################################################################
				# END: libjpeg-turbo
				################################################################################

				################################################################################
				# libwebp
				################################################################################

				Copyright (c) 2010, Google Inc. All rights reserved.

				Redistribution and use in source and binary forms, with or without
				modification, are permitted provided that the following conditions are
				met:

				  * Redistributions of source code must retain the above copyright
					notice, this list of conditions and the following disclaimer.

				  * Redistributions in binary form must reproduce the above copyright
					notice, this list of conditions and the following disclaimer in
					the documentation and/or other materials provided with the
					distribution.

				  * Neither the name of Google nor the names of its contributors may
					be used to endorse or promote products derived from this software
					without specific prior written permission.

				THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
				"AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
				LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
				A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
				HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
				SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
				LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
				DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
				THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
				(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
				OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

				################################################################################
				# END: libwebp
				################################################################################

				################################################################################
				# libmicrohttpd
				################################################################################

				Some of this code is DUAL-LICENSED.  If you use MHD without HTTPS/SSL
				support, you are free to choose between the LGPL and the eCos License
				(http://ecos.sourceware.org/license-overview.html).  If you compile
				MHD with HTTPS support, you must obey the terms of the GNU LGPL.


						  GNU LESSER GENERAL PUBLIC LICENSE
							   Version 2.1, February 1999

				 Copyright (C) 1991, 1999 Free Software Foundation, Inc.
				 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
				 Everyone is permitted to copy and distribute verbatim copies
				 of this license document, but changing it is not allowed.

				[This is the first released version of the Lesser GPL.  It also counts
				 as the successor of the GNU Library Public License, version 2, hence
				 the version number 2.1.]

								Preamble

				  The licenses for most software are designed to take away your
				freedom to share and change it.  By contrast, the GNU General Public
				Licenses are intended to guarantee your freedom to share and change
				free software--to make sure the software is free for all its users.

				  This license, the Lesser General Public License, applies to some
				specially designated software packages--typically libraries--of the
				Free Software Foundation and other authors who decide to use it.  You
				can use it too, but we suggest you first think carefully about whether
				this license or the ordinary General Public License is the better
				strategy to use in any particular case, based on the explanations below.

				  When we speak of free software, we are referring to freedom of use,
				not price.  Our General Public Licenses are designed to make sure that
				you have the freedom to distribute copies of free software (and charge
				for this service if you wish); that you receive source code or can get
				it if you want it; that you can change the software and use pieces of
				it in new free programs; and that you are informed that you can do
				these things.

				  To protect your rights, we need to make restrictions that forbid
				distributors to deny you these rights or to ask you to surrender these
				rights.  These restrictions translate to certain responsibilities for
				you if you distribute copies of the library or if you modify it.

				  For example, if you distribute copies of the library, whether gratis
				or for a fee, you must give the recipients all the rights that we gave
				you.  You must make sure that they, too, receive or can get the source
				code.  If you link other code with the library, you must provide
				complete object files to the recipients, so that they can relink them
				with the library after making changes to the library and recompiling
				it.  And you must show them these terms so they know their rights.

				  We protect your rights with a two-step method: (1) we copyright the
				library, and (2) we offer you this license, which gives you legal
				permission to copy, distribute and/or modify the library.

				  To protect each distributor, we want to make it very clear that
				there is no warranty for the free library.  Also, if the library is
				modified by someone else and passed on, the recipients should know
				that what they have is not the original version, so that the original
				author's reputation will not be affected by problems that might be
				introduced by others.

				  Finally, software patents pose a constant threat to the existence of
				any free program.  We wish to make sure that a company cannot
				effectively restrict the users of a free program by obtaining a
				restrictive license from a patent holder.  Therefore, we insist that
				any patent license obtained for a version of the library must be
				consistent with the full freedom of use specified in this license.

				  Most GNU software, including some libraries, is covered by the
				ordinary GNU General Public License.  This license, the GNU Lesser
				General Public License, applies to certain designated libraries, and
				is quite different from the ordinary General Public License.  We use
				this license for certain libraries in order to permit linking those
				libraries into non-free programs.

				  When a program is linked with a library, whether statically or using
				a shared library, the combination of the two is legally speaking a
				combined work, a derivative of the original library.  The ordinary
				General Public License therefore permits such linking only if the
				entire combination fits its criteria of freedom.  The Lesser General
				Public License permits more lax criteria for linking other code with
				the library.

				  We call this license the "Lesser" General Public License because it
				does Less to protect the user's freedom than the ordinary General
				Public License.  It also provides other free software developers Less
				of an advantage over competing non-free programs.  These disadvantages
				are the reason we use the ordinary General Public License for many
				libraries.  However, the Lesser license provides advantages in certain
				special circumstances.

				  For example, on rare occasions, there may be a special need to
				encourage the widest possible use of a certain library, so that it becomes
				a de-facto standard.  To achieve this, non-free programs must be
				allowed to use the library.  A more frequent case is that a free
				library does the same job as widely used non-free libraries.  In this
				case, there is little to gain by limiting the free library to free
				software only, so we use the Lesser General Public License.

				  In other cases, permission to use a particular library in non-free
				programs enables a greater number of people to use a large body of
				free software.  For example, permission to use the GNU C Library in
				non-free programs enables many more people to use the whole GNU
				operating system, as well as its variant, the GNU/Linux operating
				system.

				  Although the Lesser General Public License is Less protective of the
				users' freedom, it does ensure that the user of a program that is
				linked with the Library has the freedom and the wherewithal to run
				that program using a modified version of the Library.

				  The precise terms and conditions for copying, distribution and
				modification follow.  Pay close attention to the difference between a
				"work based on the library" and a "work that uses the library".  The
				former contains code derived from the library, whereas the latter must
				be combined with the library in order to run.

						  GNU LESSER GENERAL PUBLIC LICENSE
				   TERMS AND CONDITIONS FOR COPYING, DISTRIBUTION AND MODIFICATION

				  0. This License Agreement applies to any software library or other
				program which contains a notice placed by the copyright holder or
				other authorized party saying it may be distributed under the terms of
				this Lesser General Public License (also called "this License").
				Each licensee is addressed as "you".

				  A "library" means a collection of software functions and/or data
				prepared so as to be conveniently linked with application programs
				(which use some of those functions and data) to form executables.

				  The "Library", below, refers to any such software library or work
				which has been distributed under these terms.  A "work based on the
				Library" means either the Library or any derivative work under
				copyright law: that is to say, a work containing the Library or a
				portion of it, either verbatim or with modifications and/or translated
				straightforwardly into another language.  (Hereinafter, translation is
				included without limitation in the term "modification".)

				  "Source code" for a work means the preferred form of the work for
				making modifications to it.  For a library, complete source code means
				all the source code for all modules it contains, plus any associated
				interface definition files, plus the scripts used to control compilation
				and installation of the library.

				  Activities other than copying, distribution and modification are not
				covered by this License; they are outside its scope.  The act of
				running a program using the Library is not restricted, and output from
				such a program is covered only if its contents constitute a work based
				on the Library (independent of the use of the Library in a tool for
				writing it).  Whether that is true depends on what the Library does
				and what the program that uses the Library does.

				  1. You may copy and distribute verbatim copies of the Library's
				complete source code as you receive it, in any medium, provided that
				you conspicuously and appropriately publish on each copy an
				appropriate copyright notice and disclaimer of warranty; keep intact
				all the notices that refer to this License and to the absence of any
				warranty; and distribute a copy of this License along with the
				Library.

				  You may charge a fee for the physical act of transferring a copy,
				and you may at your option offer warranty protection in exchange for a
				fee.

				  2. You may modify your copy or copies of the Library or any portion
				of it, thus forming a work based on the Library, and copy and
				distribute such modifications or work under the terms of Section 1
				above, provided that you also meet all of these conditions:

					a) The modified work must itself be a software library.

					b) You must cause the files modified to carry prominent notices
					stating that you changed the files and the date of any change.

					c) You must cause the whole of the work to be licensed at no
					charge to all third parties under the terms of this License.

					d) If a facility in the modified Library refers to a function or a
					table of data to be supplied by an application program that uses
					the facility, other than as an argument passed when the facility
					is invoked, then you must make a good faith effort to ensure that,
					in the event an application does not supply such function or
					table, the facility still operates, and performs whatever part of
					its purpose remains meaningful.

					(For example, a function in a library to compute square roots has
					a purpose that is entirely well-defined independent of the
					application.  Therefore, Subsection 2d requires that any
					application-supplied function or table used by this function must
					be optional: if the application does not supply it, the square
					root function must still compute square roots.)

				These requirements apply to the modified work as a whole.  If
				identifiable sections of that work are not derived from the Library,
				and can be reasonably considered independent and separate works in
				themselves, then this License, and its terms, do not apply to those
				sections when you distribute them as separate works.  But when you
				distribute the same sections as part of a whole which is a work based
				on the Library, the distribution of the whole must be on the terms of
				this License, whose permissions for other licensees extend to the
				entire whole, and thus to each and every part regardless of who wrote
				it.

				Thus, it is not the intent of this section to claim rights or contest
				your rights to work written entirely by you; rather, the intent is to
				exercise the right to control the distribution of derivative or
				collective works based on the Library.

				In addition, mere aggregation of another work not based on the Library
				with the Library (or with a work based on the Library) on a volume of
				a storage or distribution medium does not bring the other work under
				the scope of this License.

				  3. You may opt to apply the terms of the ordinary GNU General Public
				License instead of this License to a given copy of the Library.  To do
				this, you must alter all the notices that refer to this License, so
				that they refer to the ordinary GNU General Public License, version 2,
				instead of to this License.  (If a newer version than version 2 of the
				ordinary GNU General Public License has appeared, then you can specify
				that version instead if you wish.)  Do not make any other change in
				these notices.

				  Once this change is made in a given copy, it is irreversible for
				that copy, so the ordinary GNU General Public License applies to all
				subsequent copies and derivative works made from that copy.

				  This option is useful when you wish to copy part of the code of
				the Library into a program that is not a library.

				  4. You may copy and distribute the Library (or a portion or
				derivative of it, under Section 2) in object code or executable form
				under the terms of Sections 1 and 2 above provided that you accompany
				it with the complete corresponding machine-readable source code, which
				must be distributed under the terms of Sections 1 and 2 above on a
				medium customarily used for software interchange.

				  If distribution of object code is made by offering access to copy
				from a designated place, then offering equivalent access to copy the
				source code from the same place satisfies the requirement to
				distribute the source code, even though third parties are not
				compelled to copy the source along with the object code.

				  5. A program that contains no derivative of any portion of the
				Library, but is designed to work with the Library by being compiled or
				linked with it, is called a "work that uses the Library".  Such a
				work, in isolation, is not a derivative work of the Library, and
				therefore falls outside the scope of this License.

				  However, linking a "work that uses the Library" with the Library
				creates an executable that is a derivative of the Library (because it
				contains portions of the Library), rather than a "work that uses the
				library".  The executable is therefore covered by this License.
				Section 6 states terms for distribution of such executables.

				  When a "work that uses the Library" uses material from a header file
				that is part of the Library, the object code for the work may be a
				derivative work of the Library even though the source code is not.
				Whether this is true is especially significant if the work can be
				linked without the Library, or if the work is itself a library.  The
				threshold for this to be true is not precisely defined by law.

				  If such an object file uses only numerical parameters, data
				structure layouts and accessors, and small macros and small inline
				functions (ten lines or less in length), then the use of the object
				file is unrestricted, regardless of whether it is legally a derivative
				work.  (Executables containing this object code plus portions of the
				Library will still fall under Section 6.)

				  Otherwise, if the work is a derivative of the Library, you may
				distribute the object code for the work under the terms of Section 6.
				Any executables containing that work also fall under Section 6,
				whether or not they are linked directly with the Library itself.

				  6. As an exception to the Sections above, you may also combine or
				link a "work that uses the Library" with the Library to produce a
				work containing portions of the Library, and distribute that work
				under terms of your choice, provided that the terms permit
				modification of the work for the customer's own use and reverse
				engineering for debugging such modifications.

				  You must give prominent notice with each copy of the work that the
				Library is used in it and that the Library and its use are covered by
				this License.  You must supply a copy of this License.  If the work
				during execution displays copyright notices, you must include the
				copyright notice for the Library among them, as well as a reference
				directing the user to the copy of this License.  Also, you must do one
				of these things:

					a) Accompany the work with the complete corresponding
					machine-readable source code for the Library including whatever
					changes were used in the work (which must be distributed under
					Sections 1 and 2 above); and, if the work is an executable linked
					with the Library, with the complete machine-readable "work that
					uses the Library", as object code and/or source code, so that the
					user can modify the Library and then relink to produce a modified
					executable containing the modified Library.  (It is understood
					that the user who changes the contents of definitions files in the
					Library will not necessarily be able to recompile the application
					to use the modified definitions.)

					b) Use a suitable shared library mechanism for linking with the
					Library.  A suitable mechanism is one that (1) uses at run time a
					copy of the library already present on the user's computer system,
					rather than copying library functions into the executable, and (2)
					will operate properly with a modified version of the library, if
					the user installs one, as long as the modified version is
					interface-compatible with the version that the work was made with.

					c) Accompany the work with a written offer, valid for at
					least three years, to give the same user the materials
					specified in Subsection 6a, above, for a charge no more
					than the cost of performing this distribution.

					d) If distribution of the work is made by offering access to copy
					from a designated place, offer equivalent access to copy the above
					specified materials from the same place.

					e) Verify that the user has already received a copy of these
					materials or that you have already sent this user a copy.

				  For an executable, the required form of the "work that uses the
				Library" must include any data and utility programs needed for
				reproducing the executable from it.  However, as a special exception,
				the materials to be distributed need not include anything that is
				normally distributed (in either source or binary form) with the major
				components (compiler, kernel, and so on) of the operating system on
				which the executable runs, unless that component itself accompanies
				the executable.

				  It may happen that this requirement contradicts the license
				restrictions of other proprietary libraries that do not normally
				accompany the operating system.  Such a contradiction means you cannot
				use both them and the Library together in an executable that you
				distribute.

				  7. You may place library facilities that are a work based on the
				Library side-by-side in a single library together with other library
				facilities not covered by this License, and distribute such a combined
				library, provided that the separate distribution of the work based on
				the Library and of the other library facilities is otherwise
				permitted, and provided that you do these two things:

					a) Accompany the combined library with a copy of the same work
					based on the Library, uncombined with any other library
					facilities.  This must be distributed under the terms of the
					Sections above.

					b) Give prominent notice with the combined library of the fact
					that part of it is a work based on the Library, and explaining
					where to find the accompanying uncombined form of the same work.

				  8. You may not copy, modify, sublicense, link with, or distribute
				the Library except as expressly provided under this License.  Any
				attempt otherwise to copy, modify, sublicense, link with, or
				distribute the Library is void, and will automatically terminate your
				rights under this License.  However, parties who have received copies,
				or rights, from you under this License will not have their licenses
				terminated so long as such parties remain in full compliance.

				  9. You are not required to accept this License, since you have not
				signed it.  However, nothing else grants you permission to modify or
				distribute the Library or its derivative works.  These actions are
				prohibited by law if you do not accept this License.  Therefore, by
				modifying or distributing the Library (or any work based on the
				Library), you indicate your acceptance of this License to do so, and
				all its terms and conditions for copying, distributing or modifying
				the Library or works based on it.

				  10. Each time you redistribute the Library (or any work based on the
				Library), the recipient automatically receives a license from the
				original licensor to copy, distribute, link with or modify the Library
				subject to these terms and conditions.  You may not impose any further
				restrictions on the recipients' exercise of the rights granted herein.
				You are not responsible for enforcing compliance by third parties with
				this License.

				  11. If, as a consequence of a court judgment or allegation of patent
				infringement or for any other reason (not limited to patent issues),
				conditions are imposed on you (whether by court order, agreement or
				otherwise) that contradict the conditions of this License, they do not
				excuse you from the conditions of this License.  If you cannot
				distribute so as to satisfy simultaneously your obligations under this
				License and any other pertinent obligations, then as a consequence you
				may not distribute the Library at all.  For example, if a patent
				license would not permit royalty-free redistribution of the Library by
				all those who receive copies directly or indirectly through you, then
				the only way you could satisfy both it and this License would be to
				refrain entirely from distribution of the Library.

				If any portion of this section is held invalid or unenforceable under any
				particular circumstance, the balance of the section is intended to apply,
				and the section as a whole is intended to apply in other circumstances.

				It is not the purpose of this section to induce you to infringe any
				patents or other property right claims or to contest validity of any
				such claims; this section has the sole purpose of protecting the
				integrity of the free software distribution system which is
				implemented by public license practices.  Many people have made
				generous contributions to the wide range of software distributed
				through that system in reliance on consistent application of that
				system; it is up to the author/donor to decide if he or she is willing
				to distribute software through any other system and a licensee cannot
				impose that choice.

				This section is intended to make thoroughly clear what is believed to
				be a consequence of the rest of this License.

				  12. If the distribution and/or use of the Library is restricted in
				certain countries either by patents or by copyrighted interfaces, the
				original copyright holder who places the Library under this License may add
				an explicit geographical distribution limitation excluding those countries,
				so that distribution is permitted only in or among countries not thus
				excluded.  In such case, this License incorporates the limitation as if
				written in the body of this License.

				  13. The Free Software Foundation may publish revised and/or new
				versions of the Lesser General Public License from time to time.
				Such new versions will be similar in spirit to the present version,
				but may differ in detail to address new problems or concerns.

				Each version is given a distinguishing version number.  If the Library
				specifies a version number of this License which applies to it and
				"any later version", you have the option of following the terms and
				conditions either of that version or of any later version published by
				the Free Software Foundation.  If the Library does not specify a
				license version number, you may choose any version ever published by
				the Free Software Foundation.

				  14. If you wish to incorporate parts of the Library into other free
				programs whose distribution conditions are incompatible with these,
				write to the author to ask for permission.  For software which is
				copyrighted by the Free Software Foundation, write to the Free
				Software Foundation; we sometimes make exceptions for this.  Our
				decision will be guided by the two goals of preserving the free status
				of all derivatives of our free software and of promoting the sharing
				and reuse of software generally.

								NO WARRANTY

				  15. BECAUSE THE LIBRARY IS LICENSED FREE OF CHARGE, THERE IS NO
				WARRANTY FOR THE LIBRARY, TO THE EXTENT PERMITTED BY APPLICABLE LAW.
				EXCEPT WHEN OTHERWISE STATED IN WRITING THE COPYRIGHT HOLDERS AND/OR
				OTHER PARTIES PROVIDE THE LIBRARY "AS IS" WITHOUT WARRANTY OF ANY
				KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, THE
				IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR
				PURPOSE.  THE ENTIRE RISK AS TO THE QUALITY AND PERFORMANCE OF THE
				LIBRARY IS WITH YOU.  SHOULD THE LIBRARY PROVE DEFECTIVE, YOU ASSUME
				THE COST OF ALL NECESSARY SERVICING, REPAIR OR CORRECTION.

				  16. IN NO EVENT UNLESS REQUIRED BY APPLICABLE LAW OR AGREED TO IN
				WRITING WILL ANY COPYRIGHT HOLDER, OR ANY OTHER PARTY WHO MAY MODIFY
				AND/OR REDISTRIBUTE THE LIBRARY AS PERMITTED ABOVE, BE LIABLE TO YOU
				FOR DAMAGES, INCLUDING ANY GENERAL, SPECIAL, INCIDENTAL OR
				CONSEQUENTIAL DAMAGES ARISING OUT OF THE USE OR INABILITY TO USE THE
				LIBRARY (INCLUDING BUT NOT LIMITED TO LOSS OF DATA OR DATA BEING
				RENDERED INACCURATE OR LOSSES SUSTAINED BY YOU OR THIRD PARTIES OR A
				FAILURE OF THE LIBRARY TO OPERATE WITH ANY OTHER SOFTWARE), EVEN IF
				SUCH HOLDER OR OTHER PARTY HAS BEEN ADVISED OF THE POSSIBILITY OF SUCH
				DAMAGES.

							 END OF TERMS AND CONDITIONS

						   How to Apply These Terms to Your New Libraries

				  If you develop a new library, and you want it to be of the greatest
				possible use to the public, we recommend making it free software that
				everyone can redistribute and change.  You can do so by permitting
				redistribution under these terms (or, alternatively, under the terms of the
				ordinary General Public License).

				  To apply these terms, attach the following notices to the library.  It is
				safest to attach them to the start of each source file to most effectively
				convey the exclusion of warranty; and each file should have at least the
				"copyright" line and a pointer to where the full notice is found.

					<one line to give the library's name and a brief idea of what it does.>
					Copyright (C) <year>  <name of author>

					This library is free software; you can redistribute it and/or
					modify it under the terms of the GNU Lesser General Public
					License as published by the Free Software Foundation; either
					version 2.1 of the License, or (at your option) any later version.

					This library is distributed in the hope that it will be useful,
					but WITHOUT ANY WARRANTY; without even the implied warranty of
					MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
					Lesser General Public License for more details.

					You should have received a copy of the GNU Lesser General Public
					License along with this library; if not, write to the Free Software
					Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA

				Also add information on how to contact you by electronic and paper mail.

				You should also get your employer (if you work as a programmer) or your
				school, if any, to sign a "copyright disclaimer" for the library, if
				necessary.  Here is a sample; alter the names:

				  Yoyodyne, Inc., hereby disclaims all copyright interest in the
				  library `Frob' (a library for tweaking knobs) written by James Random Hacker.

				  <signature of Ty Coon>, 1 April 1990
				  Ty Coon, President of Vice

				That's all there is to it!

				################################################################################
				# END: libmicrohttpd
				################################################################################

				################################################################################
				# piex
				################################################################################

												 Apache License
										   Version 2.0, January 2004
										http://www.apache.org/licenses/

				   TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION

				   1. Definitions.

					  "License" shall mean the terms and conditions for use, reproduction,
					  and distribution as defined by Sections 1 through 9 of this document.

					  "Licensor" shall mean the copyright owner or entity authorized by
					  the copyright owner that is granting the License.

					  "Legal Entity" shall mean the union of the acting entity and all
					  other entities that control, are controlled by, or are under common
					  control with that entity. For the purposes of this definition,
					  "control" means (i) the power, direct or indirect, to cause the
					  direction or management of such entity, whether by contract or
					  otherwise, or (ii) ownership of fifty percent (50%) or more of the
					  outstanding shares, or (iii) beneficial ownership of such entity.

					  "You" (or "Your") shall mean an individual or Legal Entity
					  exercising permissions granted by this License.

					  "Source" form shall mean the preferred form for making modifications,
					  including but not limited to software source code, documentation
					  source, and configuration files.

					  "Object" form shall mean any form resulting from mechanical
					  transformation or translation of a Source form, including but
					  not limited to compiled object code, generated documentation,
					  and conversions to other media types.

					  "Work" shall mean the work of authorship, whether in Source or
					  Object form, made available under the License, as indicated by a
					  copyright notice that is included in or attached to the work
					  (an example is provided in the Appendix below).

					  "Derivative Works" shall mean any work, whether in Source or Object
					  form, that is based on (or derived from) the Work and for which the
					  editorial revisions, annotations, elaborations, or other modifications
					  represent, as a whole, an original work of authorship. For the purposes
					  of this License, Derivative Works shall not include works that remain
					  separable from, or merely link (or bind by name) to the interfaces of,
					  the Work and Derivative Works thereof.

					  "Contribution" shall mean any work of authorship, including
					  the original version of the Work and any modifications or additions
					  to that Work or Derivative Works thereof, that is intentionally
					  submitted to Licensor for inclusion in the Work by the copyright owner
					  or by an individual or Legal Entity authorized to submit on behalf of
					  the copyright owner. For the purposes of this definition, "submitted"
					  means any form of electronic, verbal, or written communication sent
					  to the Licensor or its representatives, including but not limited to
					  communication on electronic mailing lists, source code control systems,
					  and issue tracking systems that are managed by, or on behalf of, the
					  Licensor for the purpose of discussing and improving the Work, but
					  excluding communication that is conspicuously marked or otherwise
					  designated in writing by the copyright owner as "Not a Contribution."

					  "Contributor" shall mean Licensor and any individual or Legal Entity
					  on behalf of whom a Contribution has been received by Licensor and
					  subsequently incorporated within the Work.

				   2. Grant of Copyright License. Subject to the terms and conditions of
					  this License, each Contributor hereby grants to You a perpetual,
					  worldwide, non-exclusive, no-charge, royalty-free, irrevocable
					  copyright license to reproduce, prepare Derivative Works of,
					  publicly display, publicly perform, sublicense, and distribute the
					  Work and such Derivative Works in Source or Object form.

				   3. Grant of Patent License. Subject to the terms and conditions of
					  this License, each Contributor hereby grants to You a perpetual,
					  worldwide, non-exclusive, no-charge, royalty-free, irrevocable
					  (except as stated in this section) patent license to make, have made,
					  use, offer to sell, sell, import, and otherwise transfer the Work,
					  where such license applies only to those patent claims licensable
					  by such Contributor that are necessarily infringed by their
					  Contribution(s) alone or by combination of their Contribution(s)
					  with the Work to which such Contribution(s) was submitted. If You
					  institute patent litigation against any entity (including a
					  cross-claim or counterclaim in a lawsuit) alleging that the Work
					  or a Contribution incorporated within the Work constitutes direct
					  or contributory patent infringement, then any patent licenses
					  granted to You under this License for that Work shall terminate
					  as of the date such litigation is filed.

				   4. Redistribution. You may reproduce and distribute copies of the
					  Work or Derivative Works thereof in any medium, with or without
					  modifications, and in Source or Object form, provided that You
					  meet the following conditions:

					  (a) You must give any other recipients of the Work or
						  Derivative Works a copy of this License; and

					  (b) You must cause any modified files to carry prominent notices
						  stating that You changed the files; and

					  (c) You must retain, in the Source form of any Derivative Works
						  that You distribute, all copyright, patent, trademark, and
						  attribution notices from the Source form of the Work,
						  excluding those notices that do not pertain to any part of
						  the Derivative Works; and

					  (d) If the Work includes a "NOTICE" text file as part of its
						  distribution, then any Derivative Works that You distribute must
						  include a readable copy of the attribution notices contained
						  within such NOTICE file, excluding those notices that do not
						  pertain to any part of the Derivative Works, in at least one
						  of the following places: within a NOTICE text file distributed
						  as part of the Derivative Works; within the Source form or
						  documentation, if provided along with the Derivative Works; or,
						  within a display generated by the Derivative Works, if and
						  wherever such third-party notices normally appear. The contents
						  of the NOTICE file are for informational purposes only and
						  do not modify the License. You may add Your own attribution
						  notices within Derivative Works that You distribute, alongside
						  or as an addendum to the NOTICE text from the Work, provided
						  that such additional attribution notices cannot be construed
						  as modifying the License.

					  You may add Your own copyright statement to Your modifications and
					  may provide additional or different license terms and conditions
					  for use, reproduction, or distribution of Your modifications, or
					  for any such Derivative Works as a whole, provided Your use,
					  reproduction, and distribution of the Work otherwise complies with
					  the conditions stated in this License.

				   5. Submission of Contributions. Unless You explicitly state otherwise,
					  any Contribution intentionally submitted for inclusion in the Work
					  by You to the Licensor shall be under the terms and conditions of
					  this License, without any additional terms or conditions.
					  Notwithstanding the above, nothing herein shall supersede or modify
					  the terms of any separate license agreement you may have executed
					  with Licensor regarding such Contributions.

				   6. Trademarks. This License does not grant permission to use the trade
					  names, trademarks, service marks, or product names of the Licensor,
					  except as required for reasonable and customary use in describing the
					  origin of the Work and reproducing the content of the NOTICE file.

				   7. Disclaimer of Warranty. Unless required by applicable law or
					  agreed to in writing, Licensor provides the Work (and each
					  Contributor provides its Contributions) on an "AS IS" BASIS,
					  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or
					  implied, including, without limitation, any warranties or conditions
					  of TITLE, NON-INFRINGEMENT, MERCHANTABILITY, or FITNESS FOR A
					  PARTICULAR PURPOSE. You are solely responsible for determining the
					  appropriateness of using or redistributing the Work and assume any
					  risks associated with Your exercise of permissions under this License.

				   8. Limitation of Liability. In no event and under no legal theory,
					  whether in tort (including negligence), contract, or otherwise,
					  unless required by applicable law (such as deliberate and grossly
					  negligent acts) or agreed to in writing, shall any Contributor be
					  liable to You for damages, including any direct, indirect, special,
					  incidental, or consequential damages of any character arising as a
					  result of this License or out of the use or inability to use the
					  Work (including but not limited to damages for loss of goodwill,
					  work stoppage, computer failure or malfunction, or any and all
					  other commercial damages or losses), even if such Contributor
					  has been advised of the possibility of such damages.

				   9. Accepting Warranty or Additional Liability. While redistributing
					  the Work or Derivative Works thereof, You may choose to offer,
					  and charge a fee for, acceptance of support, warranty, indemnity,
					  or other liability obligations and/or rights consistent with this
					  License. However, in accepting such obligations, You may act only
					  on Your own behalf and on Your sole responsibility, not on behalf
					  of any other Contributor, and only if You agree to indemnify,
					  defend, and hold each Contributor harmless for any liability
					  incurred by, or claims asserted against, such Contributor by reason
					  of your accepting any such warranty or additional liability.

				   END OF TERMS AND CONDITIONS

				   APPENDIX: How to apply the Apache License to your work.

					  To apply the Apache License to your work, attach the following
					  boilerplate notice, with the fields enclosed by brackets "[]"
					  replaced with your own identifying information. (Don't include
					  the brackets!)  The text should be enclosed in the appropriate
					  comment syntax for the file format. We also recommend that a
					  file or class name and description of purpose be included on the
					  same "printed page" as the copyright notice for easier
					  identification within third-party archives.

				   Copyright [yyyy] [name of copyright owner]

				   Licensed under the Apache License, Version 2.0 (the "License");
				   you may not use this file except in compliance with the License.
				   You may obtain a copy of the License at

					   http://www.apache.org/licenses/LICENSE-2.0

				   Unless required by applicable law or agreed to in writing, software
				   distributed under the License is distributed on an "AS IS" BASIS,
				   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
				   See the License for the specific language governing permissions and
				   limitations under the License.

				################################################################################
				# END: piex
				################################################################################

				################################################################################
				# sdl
				################################################################################

				Simple DirectMedia Layer
				Copyright (C) 1997-2015 Sam Lantinga <slouken@libsdl.org>

				This software is provided 'as-is', without any express or implied
				warranty.  In no event will the authors be held liable for any damages
				arising from the use of this software.

				Permission is granted to anyone to use this software for any purpose,
				including commercial applications, and to alter it and redistribute it
				freely, subject to the following restrictions:

				1. The origin of this software must not be misrepresented; you must not
				   claim that you wrote the original software. If you use this software
				   in a product, an acknowledgment in the product documentation would be
				   appreciated but is not required. 
				2. Altered source versions must be plainly marked as such, and must not be
				   misrepresented as being the original software.
				3. This notice may not be removed or altered from any source distribution.

				################################################################################
				# END: sdl
				################################################################################

				################################################################################
				# sfntly
				################################################################################

												 Apache License
										   Version 2.0, January 2004
										http://www.apache.org/licenses/

				   TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION

				   1. Definitions.

					  "License" shall mean the terms and conditions for use, reproduction,
					  and distribution as defined by Sections 1 through 9 of this document.

					  "Licensor" shall mean the copyright owner or entity authorized by
					  the copyright owner that is granting the License.

					  "Legal Entity" shall mean the union of the acting entity and all
					  other entities that control, are controlled by, or are under common
					  control with that entity. For the purposes of this definition,
					  "control" means (i) the power, direct or indirect, to cause the
					  direction or management of such entity, whether by contract or
					  otherwise, or (ii) ownership of fifty percent (50%) or more of the
					  outstanding shares, or (iii) beneficial ownership of such entity.

					  "You" (or "Your") shall mean an individual or Legal Entity
					  exercising permissions granted by this License.

					  "Source" form shall mean the preferred form for making modifications,
					  including but not limited to software source code, documentation
					  source, and configuration files.

					  "Object" form shall mean any form resulting from mechanical
					  transformation or translation of a Source form, including but
					  not limited to compiled object code, generated documentation,
					  and conversions to other media types.

					  "Work" shall mean the work of authorship, whether in Source or
					  Object form, made available under the License, as indicated by a
					  copyright notice that is included in or attached to the work
					  (an example is provided in the Appendix below).

					  "Derivative Works" shall mean any work, whether in Source or Object
					  form, that is based on (or derived from) the Work and for which the
					  editorial revisions, annotations, elaborations, or other modifications
					  represent, as a whole, an original work of authorship. For the purposes
					  of this License, Derivative Works shall not include works that remain
					  separable from, or merely link (or bind by name) to the interfaces of,
					  the Work and Derivative Works thereof.

					  "Contribution" shall mean any work of authorship, including
					  the original version of the Work and any modifications or additions
					  to that Work or Derivative Works thereof, that is intentionally
					  submitted to Licensor for inclusion in the Work by the copyright owner
					  or by an individual or Legal Entity authorized to submit on behalf of
					  the copyright owner. For the purposes of this definition, "submitted"
					  means any form of electronic, verbal, or written communication sent
					  to the Licensor or its representatives, including but not limited to
					  communication on electronic mailing lists, source code control systems,
					  and issue tracking systems that are managed by, or on behalf of, the
					  Licensor for the purpose of discussing and improving the Work, but
					  excluding communication that is conspicuously marked or otherwise
					  designated in writing by the copyright owner as "Not a Contribution."

					  "Contributor" shall mean Licensor and any individual or Legal Entity
					  on behalf of whom a Contribution has been received by Licensor and
					  subsequently incorporated within the Work.

				   2. Grant of Copyright License. Subject to the terms and conditions of
					  this License, each Contributor hereby grants to You a perpetual,
					  worldwide, non-exclusive, no-charge, royalty-free, irrevocable
					  copyright license to reproduce, prepare Derivative Works of,
					  publicly display, publicly perform, sublicense, and distribute the
					  Work and such Derivative Works in Source or Object form.

				   3. Grant of Patent License. Subject to the terms and conditions of
					  this License, each Contributor hereby grants to You a perpetual,
					  worldwide, non-exclusive, no-charge, royalty-free, irrevocable
					  (except as stated in this section) patent license to make, have made,
					  use, offer to sell, sell, import, and otherwise transfer the Work,
					  where such license applies only to those patent claims licensable
					  by such Contributor that are necessarily infringed by their
					  Contribution(s) alone or by combination of their Contribution(s)
					  with the Work to which such Contribution(s) was submitted. If You
					  institute patent litigation against any entity (including a
					  cross-claim or counterclaim in a lawsuit) alleging that the Work
					  or a Contribution incorporated within the Work constitutes direct
					  or contributory patent infringement, then any patent licenses
					  granted to You under this License for that Work shall terminate
					  as of the date such litigation is filed.

				   4. Redistribution. You may reproduce and distribute copies of the
					  Work or Derivative Works thereof in any medium, with or without
					  modifications, and in Source or Object form, provided that You
					  meet the following conditions:

					  (a) You must give any other recipients of the Work or
						  Derivative Works a copy of this License; and

					  (b) You must cause any modified files to carry prominent notices
						  stating that You changed the files; and

					  (c) You must retain, in the Source form of any Derivative Works
						  that You distribute, all copyright, patent, trademark, and
						  attribution notices from the Source form of the Work,
						  excluding those notices that do not pertain to any part of
						  the Derivative Works; and

					  (d) If the Work includes a "NOTICE" text file as part of its
						  distribution, then any Derivative Works that You distribute must
						  include a readable copy of the attribution notices contained
						  within such NOTICE file, excluding those notices that do not
						  pertain to any part of the Derivative Works, in at least one
						  of the following places: within a NOTICE text file distributed
						  as part of the Derivative Works; within the Source form or
						  documentation, if provided along with the Derivative Works; or,
						  within a display generated by the Derivative Works, if and
						  wherever such third-party notices normally appear. The contents
						  of the NOTICE file are for informational purposes only and
						  do not modify the License. You may add Your own attribution
						  notices within Derivative Works that You distribute, alongside
						  or as an addendum to the NOTICE text from the Work, provided
						  that such additional attribution notices cannot be construed
						  as modifying the License.

					  You may add Your own copyright statement to Your modifications and
					  may provide additional or different license terms and conditions
					  for use, reproduction, or distribution of Your modifications, or
					  for any such Derivative Works as a whole, provided Your use,
					  reproduction, and distribution of the Work otherwise complies with
					  the conditions stated in this License.

				   5. Submission of Contributions. Unless You explicitly state otherwise,
					  any Contribution intentionally submitted for inclusion in the Work
					  by You to the Licensor shall be under the terms and conditions of
					  this License, without any additional terms or conditions.
					  Notwithstanding the above, nothing herein shall supersede or modify
					  the terms of any separate license agreement you may have executed
					  with Licensor regarding such Contributions.

				   6. Trademarks. This License does not grant permission to use the trade
					  names, trademarks, service marks, or product names of the Licensor,
					  except as required for reasonable and customary use in describing the
					  origin of the Work and reproducing the content of the NOTICE file.

				   7. Disclaimer of Warranty. Unless required by applicable law or
					  agreed to in writing, Licensor provides the Work (and each
					  Contributor provides its Contributions) on an "AS IS" BASIS,
					  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or
					  implied, including, without limitation, any warranties or conditions
					  of TITLE, NON-INFRINGEMENT, MERCHANTABILITY, or FITNESS FOR A
					  PARTICULAR PURPOSE. You are solely responsible for determining the
					  appropriateness of using or redistributing the Work and assume any
					  risks associated with Your exercise of permissions under this License.

				   8. Limitation of Liability. In no event and under no legal theory,
					  whether in tort (including negligence), contract, or otherwise,
					  unless required by applicable law (such as deliberate and grossly
					  negligent acts) or agreed to in writing, shall any Contributor be
					  liable to You for damages, including any direct, indirect, special,
					  incidental, or consequential damages of any character arising as a
					  result of this License or out of the use or inability to use the
					  Work (including but not limited to damages for loss of goodwill,
					  work stoppage, computer failure or malfunction, or any and all
					  other commercial damages or losses), even if such Contributor
					  has been advised of the possibility of such damages.

				   9. Accepting Warranty or Additional Liability. While redistributing
					  the Work or Derivative Works thereof, You may choose to offer,
					  and charge a fee for, acceptance of support, warranty, indemnity,
					  or other liability obligations and/or rights consistent with this
					  License. However, in accepting such obligations, You may act only
					  on Your own behalf and on Your sole responsibility, not on behalf
					  of any other Contributor, and only if You agree to indemnify,
					  defend, and hold each Contributor harmless for any liability
					  incurred by, or claims asserted against, such Contributor by reason
					  of your accepting any such warranty or additional liability.

				   END OF TERMS AND CONDITIONS

				   APPENDIX: How to apply the Apache License to your work.

					  To apply the Apache License to your work, attach the following
					  boilerplate notice, with the fields enclosed by brackets "[]"
					  replaced with your own identifying information. (Don't include
					  the brackets!)  The text should be enclosed in the appropriate
					  comment syntax for the file format. We also recommend that a
					  file or class name and description of purpose be included on the
					  same "printed page" as the copyright notice for easier
					  identification within third-party archives.

				   Copyright 2011 Google Inc. All Rights Reserved.

				   Licensed under the Apache License, Version 2.0 (the "License");
				   you may not use this file except in compliance with the License.
				   You may obtain a copy of the License at

					   http://www.apache.org/licenses/LICENSE-2.0

				   Unless required by applicable law or agreed to in writing, software
				   distributed under the License is distributed on an "AS IS" BASIS,
				   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
				   See the License for the specific language governing permissions and
				   limitations under the License.

				################################################################################
				# END: sfntly
				################################################################################

				################################################################################
				# SPIR-V Headers
				################################################################################

				Copyright (c) 2015-2016 The Khronos Group Inc.

				Permission is hereby granted, free of charge, to any person obtaining a
				copy of this software and/or associated documentation files (the
				"Materials"), to deal in the Materials without restriction, including
				without limitation the rights to use, copy, modify, merge, publish,
				distribute, sublicense, and/or sell copies of the Materials, and to
				permit persons to whom the Materials are furnished to do so, subject to
				the following conditions:

				The above copyright notice and this permission notice shall be included
				in all copies or substantial portions of the Materials.

				MODIFICATIONS TO THIS FILE MAY MEAN IT NO LONGER ACCURATELY REFLECTS
				KHRONOS STANDARDS. THE UNMODIFIED, NORMATIVE VERSIONS OF KHRONOS
				SPECIFICATIONS AND HEADER INFORMATION ARE LOCATED AT
				   https://www.khronos.org/registry/

				THE MATERIALS ARE PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
				EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
				MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
				IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
				CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
				TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
				MATERIALS OR THE USE OR OTHER DEALINGS IN THE MATERIALS.

				################################################################################
				# END: SPIR-V Headers
				################################################################################

				################################################################################
				# SPIR-V Tools
				################################################################################

												 Apache License
										   Version 2.0, January 2004
										http://www.apache.org/licenses/

				   TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION

				   1. Definitions.

					  "License" shall mean the terms and conditions for use, reproduction,
					  and distribution as defined by Sections 1 through 9 of this document.

					  "Licensor" shall mean the copyright owner or entity authorized by
					  the copyright owner that is granting the License.

					  "Legal Entity" shall mean the union of the acting entity and all
					  other entities that control, are controlled by, or are under common
					  control with that entity. For the purposes of this definition,
					  "control" means (i) the power, direct or indirect, to cause the
					  direction or management of such entity, whether by contract or
					  otherwise, or (ii) ownership of fifty percent (50%) or more of the
					  outstanding shares, or (iii) beneficial ownership of such entity.

					  "You" (or "Your") shall mean an individual or Legal Entity
					  exercising permissions granted by this License.

					  "Source" form shall mean the preferred form for making modifications,
					  including but not limited to software source code, documentation
					  source, and configuration files.

					  "Object" form shall mean any form resulting from mechanical
					  transformation or translation of a Source form, including but
					  not limited to compiled object code, generated documentation,
					  and conversions to other media types.

					  "Work" shall mean the work of authorship, whether in Source or
					  Object form, made available under the License, as indicated by a
					  copyright notice that is included in or attached to the work
					  (an example is provided in the Appendix below).

					  "Derivative Works" shall mean any work, whether in Source or Object
					  form, that is based on (or derived from) the Work and for which the
					  editorial revisions, annotations, elaborations, or other modifications
					  represent, as a whole, an original work of authorship. For the purposes
					  of this License, Derivative Works shall not include works that remain
					  separable from, or merely link (or bind by name) to the interfaces of,
					  the Work and Derivative Works thereof.

					  "Contribution" shall mean any work of authorship, including
					  the original version of the Work and any modifications or additions
					  to that Work or Derivative Works thereof, that is intentionally
					  submitted to Licensor for inclusion in the Work by the copyright owner
					  or by an individual or Legal Entity authorized to submit on behalf of
					  the copyright owner. For the purposes of this definition, "submitted"
					  means any form of electronic, verbal, or written communication sent
					  to the Licensor or its representatives, including but not limited to
					  communication on electronic mailing lists, source code control systems,
					  and issue tracking systems that are managed by, or on behalf of, the
					  Licensor for the purpose of discussing and improving the Work, but
					  excluding communication that is conspicuously marked or otherwise
					  designated in writing by the copyright owner as "Not a Contribution."

					  "Contributor" shall mean Licensor and any individual or Legal Entity
					  on behalf of whom a Contribution has been received by Licensor and
					  subsequently incorporated within the Work.

				   2. Grant of Copyright License. Subject to the terms and conditions of
					  this License, each Contributor hereby grants to You a perpetual,
					  worldwide, non-exclusive, no-charge, royalty-free, irrevocable
					  copyright license to reproduce, prepare Derivative Works of,
					  publicly display, publicly perform, sublicense, and distribute the
					  Work and such Derivative Works in Source or Object form.

				   3. Grant of Patent License. Subject to the terms and conditions of
					  this License, each Contributor hereby grants to You a perpetual,
					  worldwide, non-exclusive, no-charge, royalty-free, irrevocable
					  (except as stated in this section) patent license to make, have made,
					  use, offer to sell, sell, import, and otherwise transfer the Work,
					  where such license applies only to those patent claims licensable
					  by such Contributor that are necessarily infringed by their
					  Contribution(s) alone or by combination of their Contribution(s)
					  with the Work to which such Contribution(s) was submitted. If You
					  institute patent litigation against any entity (including a
					  cross-claim or counterclaim in a lawsuit) alleging that the Work
					  or a Contribution incorporated within the Work constitutes direct
					  or contributory patent infringement, then any patent licenses
					  granted to You under this License for that Work shall terminate
					  as of the date such litigation is filed.

				   4. Redistribution. You may reproduce and distribute copies of the
					  Work or Derivative Works thereof in any medium, with or without
					  modifications, and in Source or Object form, provided that You
					  meet the following conditions:

					  (a) You must give any other recipients of the Work or
						  Derivative Works a copy of this License; and

					  (b) You must cause any modified files to carry prominent notices
						  stating that You changed the files; and

					  (c) You must retain, in the Source form of any Derivative Works
						  that You distribute, all copyright, patent, trademark, and
						  attribution notices from the Source form of the Work,
						  excluding those notices that do not pertain to any part of
						  the Derivative Works; and

					  (d) If the Work includes a "NOTICE" text file as part of its
						  distribution, then any Derivative Works that You distribute must
						  include a readable copy of the attribution notices contained
						  within such NOTICE file, excluding those notices that do not
						  pertain to any part of the Derivative Works, in at least one
						  of the following places: within a NOTICE text file distributed
						  as part of the Derivative Works; within the Source form or
						  documentation, if provided along with the Derivative Works; or,
						  within a display generated by the Derivative Works, if and
						  wherever such third-party notices normally appear. The contents
						  of the NOTICE file are for informational purposes only and
						  do not modify the License. You may add Your own attribution
						  notices within Derivative Works that You distribute, alongside
						  or as an addendum to the NOTICE text from the Work, provided
						  that such additional attribution notices cannot be construed
						  as modifying the License.

					  You may add Your own copyright statement to Your modifications and
					  may provide additional or different license terms and conditions
					  for use, reproduction, or distribution of Your modifications, or
					  for any such Derivative Works as a whole, provided Your use,
					  reproduction, and distribution of the Work otherwise complies with
					  the conditions stated in this License.

				   5. Submission of Contributions. Unless You explicitly state otherwise,
					  any Contribution intentionally submitted for inclusion in the Work
					  by You to the Licensor shall be under the terms and conditions of
					  this License, without any additional terms or conditions.
					  Notwithstanding the above, nothing herein shall supersede or modify
					  the terms of any separate license agreement you may have executed
					  with Licensor regarding such Contributions.

				   6. Trademarks. This License does not grant permission to use the trade
					  names, trademarks, service marks, or product names of the Licensor,
					  except as required for reasonable and customary use in describing the
					  origin of the Work and reproducing the content of the NOTICE file.

				   7. Disclaimer of Warranty. Unless required by applicable law or
					  agreed to in writing, Licensor provides the Work (and each
					  Contributor provides its Contributions) on an "AS IS" BASIS,
					  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or
					  implied, including, without limitation, any warranties or conditions
					  of TITLE, NON-INFRINGEMENT, MERCHANTABILITY, or FITNESS FOR A
					  PARTICULAR PURPOSE. You are solely responsible for determining the
					  appropriateness of using or redistributing the Work and assume any
					  risks associated with Your exercise of permissions under this License.

				   8. Limitation of Liability. In no event and under no legal theory,
					  whether in tort (including negligence), contract, or otherwise,
					  unless required by applicable law (such as deliberate and grossly
					  negligent acts) or agreed to in writing, shall any Contributor be
					  liable to You for damages, including any direct, indirect, special,
					  incidental, or consequential damages of any character arising as a
					  result of this License or out of the use or inability to use the
					  Work (including but not limited to damages for loss of goodwill,
					  work stoppage, computer failure or malfunction, or any and all
					  other commercial damages or losses), even if such Contributor
					  has been advised of the possibility of such damages.

				   9. Accepting Warranty or Additional Liability. While redistributing
					  the Work or Derivative Works thereof, You may choose to offer,
					  and charge a fee for, acceptance of support, warranty, indemnity,
					  or other liability obligations and/or rights consistent with this
					  License. However, in accepting such obligations, You may act only
					  on Your own behalf and on Your sole responsibility, not on behalf
					  of any other Contributor, and only if You agree to indemnify,
					  defend, and hold each Contributor harmless for any liability
					  incurred by, or claims asserted against, such Contributor by reason
					  of your accepting any such warranty or additional liability.

				   END OF TERMS AND CONDITIONS

				   APPENDIX: How to apply the Apache License to your work.

					  To apply the Apache License to your work, attach the following
					  boilerplate notice, with the fields enclosed by brackets "[]"
					  replaced with your own identifying information. (Don't include
					  the brackets!)  The text should be enclosed in the appropriate
					  comment syntax for the file format. We also recommend that a
					  file or class name and description of purpose be included on the
					  same "printed page" as the copyright notice for easier
					  identification within third-party archives.

				   Copyright [yyyy] [name of copyright owner]

				   Licensed under the Apache License, Version 2.0 (the "License");
				   you may not use this file except in compliance with the License.
				   You may obtain a copy of the License at

					   http://www.apache.org/licenses/LICENSE-2.0

				   Unless required by applicable law or agreed to in writing, software
				   distributed under the License is distributed on an "AS IS" BASIS,
				   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
				   See the License for the specific language governing permissions and
				   limitations under the License.

				################################################################################
				# END: SPIR-V Tools
				################################################################################

				################################################################################
				# zlib
				################################################################################

				/* zlib.h -- interface of the 'zlib' general purpose compression library
				  version 1.2.4, March 14th, 2010

				  Copyright (C) 1995-2010 Jean-loup Gailly and Mark Adler

				  This software is provided 'as-is', without any express or implied
				  warranty.  In no event will the authors be held liable for any damages
				  arising from the use of this software.

				  Permission is granted to anyone to use this software for any purpose,
				  including commercial applications, and to alter it and redistribute it
				  freely, subject to the following restrictions:

				  1. The origin of this software must not be misrepresented; you must not
					 claim that you wrote the original software. If you use this software
					 in a product, an acknowledgment in the product documentation would be
					 appreciated but is not required.
				  2. Altered source versions must be plainly marked as such, and must not be
					 misrepresented as being the original software.
				  3. This notice may not be removed or altered from any source distribution.

				  Jean-loup Gailly
				  Mark Adler

				*/

				mozzconf.h is distributed under the MPL 1.1/GPL 2.0/LGPL 2.1 tri-license.

				################################################################################
				# END: zlib
				################################################################################
				""""
					.Split(NewlineChars, StringSplitOptions.None)
					.Select(CleanLine)
					.ToArray());

		private static readonly string[] NewlineChars = ["\n", "\r\n"];
		private static readonly char[] SlashTrimChars = ['/', ' '];
		private static readonly char[] HashTrimChars = ['#', ' '];
		private static readonly char[] StarTrimChars = ['*', ' '];

		private static string CleanLine(string line)
		{
			line = line.TrimStart();

			if (line.StartsWith("//"))
				return line.Trim(SlashTrimChars);
			if ((line.StartsWith("# ") || line == "#") && !line.StartsWith("##"))
				return line.Trim(HashTrimChars);
			if (line.StartsWith("* ") || line == "*")
				return line.Trim(StarTrimChars);
			return line.Trim(' ');
		}
	}
}
