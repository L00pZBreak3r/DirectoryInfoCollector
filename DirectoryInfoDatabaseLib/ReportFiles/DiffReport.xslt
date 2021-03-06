<?xml version="1.0" encoding="utf-16" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt">
<xsl:output method="html" encoding="utf-16" doctype-system="about:legacy-compat"/>
    <xsl:template match="##XML_ELEMENT_DIRECTORY##">
      <xsl:variable name="dir-id" select="generate-id(.)"/>
      <xsl:variable name="all-files" select="count(.//##XML_ELEMENT_FILE##)"/>
      <xsl:variable name="all-dirs" select="count(.//##XML_ELEMENT_DIRECTORY##)"/>
      <tr class="row">
        <td class="content" title="Directory name">
          <img alt="expand/collapse section" class="expandable" height="11" onclick="changepic()" src="##XSLT_SHEET_DIRECTORY_NAME##/##XSLT_SHEET_IMAGE_PLUS_NAME##" width="9" data-child="src{$dir-id}" />
          &#32;<xsl:value-of select="@##XML_ELEMENT_ATTRIBUTE_NAME##"/>
        </td>
        <td class="content" title="Directory status">
          <xsl:for-each select="##XML_ELEMENT_ALTERED##/@*">
            <xsl:variable name="prop-name" select="name(.)"/>
            <xsl:choose>
              <xsl:when test="$prop-name='##XML_ELEMENT_ALTERED_ATTRIBUTE_ACTION##'">
                <b><xsl:value-of select="$prop-name"/>=<xsl:value-of select="."/></b><br/>
              </xsl:when>
              <xsl:otherwise>
                <xsl:value-of select="$prop-name"/>=##DIFF_REPORT_DATEFORMAT_XSLTTEXT##<br/>
              </xsl:otherwise>
            </xsl:choose>
          </xsl:for-each>
        </td>
        <xsl:for-each select="@*">
          <xsl:variable name="prop-name" select="name(.)"/>
          <xsl:if test="not($prop-name='##XML_ELEMENT_ATTRIBUTE_NAME##')">
            <xsl:choose>
              <xsl:when test="$prop-name='##XML_ELEMENT_ATTRIBUTE_FILES##'">
                <td class="content" title="{$prop-name}"><xsl:value-of select="."/>(<xsl:value-of select="$all-files"/>)</td>
              </xsl:when>
              <xsl:when test="$prop-name='##XML_ELEMENT_ATTRIBUTE_DIRECTORIES##'">
                <td class="content" title="{$prop-name}"><xsl:value-of select="."/>(<xsl:value-of select="$all-dirs"/>)</td>
              </xsl:when>
              <xsl:otherwise>
                <td class="content" title="{$prop-name}">##DIFF_REPORT_DATEFORMAT_XSLTTEXT##</td>
              </xsl:otherwise>
            </xsl:choose>
          </xsl:if>
        </xsl:for-each>
      </tr>
      <tr class="collapsed" style="background-color:white;" id="src{$dir-id}">
        <td colspan="7">
          <table style="width:98%;" class="issuetable">
            <xsl:variable name="dp-cnt" select="count(##XML_ELEMENT_DIRECTORY##[1]/@*)"/>
            <xsl:variable name="fp-cnt" select="$dp-cnt - count(##XML_ELEMENT_FILE##[1]/@*)"/>
            <xsl:for-each select="##XML_ELEMENT_DIRECTORY##">
              <xsl:apply-templates select="."/>
            </xsl:for-each>
            <xsl:for-each select="##XML_ELEMENT_FILE##">
              <tr>
                <td class="issuenone" title="File name">
                  <xsl:choose>
                    <xsl:when test="@##XML_ELEMENT_ATTRIBUTE_FILESIGVERIFIED##='##XML_ELEMENT_ATTRIBUTE_FILESIGVERIFIED_SUCCESS##'">
                      <span style="color:green"><xsl:value-of select="@##XML_ELEMENT_ATTRIBUTE_NAME##"/></span>
                    </xsl:when>
                    <xsl:when test="@##XML_ELEMENT_ATTRIBUTE_FILESIGVERIFIED##='##XML_ELEMENT_ATTRIBUTE_FILESIGVERIFIED_FILEERROR##' or @##XML_ELEMENT_ATTRIBUTE_FILESIGVERIFIED##='##XML_ELEMENT_ATTRIBUTE_FILESIGVERIFIED_FORMUNKNOWN##' or @##XML_ELEMENT_ATTRIBUTE_FILESIGVERIFIED##='##XML_ELEMENT_ATTRIBUTE_FILESIGVERIFIED_NOSIGNATURE##'">
                      <xsl:value-of select="@##XML_ELEMENT_ATTRIBUTE_NAME##"/>
                    </xsl:when>
                    <xsl:otherwise>
                      <span style="color:red"><xsl:value-of select="@##XML_ELEMENT_ATTRIBUTE_NAME##"/></span>
                    </xsl:otherwise>
                  </xsl:choose>
                </td>
                <td class="issuenone" title="File status">
                  <xsl:for-each select="##XML_ELEMENT_ALTERED##/@*">
                    <xsl:variable name="prop-name" select="name(.)"/>
                    <xsl:choose>
                      <xsl:when test="$prop-name='##XML_ELEMENT_ALTERED_ATTRIBUTE_ACTION##'">
                        <b><xsl:value-of select="$prop-name"/>=<xsl:value-of select="."/></b><br/>
                      </xsl:when>
                      <xsl:otherwise>
                        <xsl:value-of select="$prop-name"/>=##DIFF_REPORT_DATEFORMAT_XSLTTEXT##<br/>
                      </xsl:otherwise>
                    </xsl:choose>
                  </xsl:for-each>
                </td>
                <xsl:for-each select="@*">
                  <xsl:variable name="prop-name" select="name(.)"/>
                  <xsl:if test="not($prop-name='##XML_ELEMENT_ATTRIBUTE_NAME##')">
                    <td class="issuenone" title="{$prop-name}">
                    <xsl:choose>
                      <xsl:when test="$prop-name='##XML_ELEMENT_ATTRIBUTE_FILESIGVERIFIED##'">
                        <xsl:choose>
                          <xsl:when test=".='##XML_ELEMENT_ATTRIBUTE_FILESIGVERIFIED_SUCCESS##'">
                            <span style="color:green">##DIFF_REPORT_DATEFORMAT_XSLTTEXT##</span>
                          </xsl:when>
                          <xsl:when test=".='##XML_ELEMENT_ATTRIBUTE_FILESIGVERIFIED_FILEERROR##' or .='##XML_ELEMENT_ATTRIBUTE_FILESIGVERIFIED_FORMUNKNOWN##' or .='##XML_ELEMENT_ATTRIBUTE_FILESIGVERIFIED_NOSIGNATURE##'">
                          ##DIFF_REPORT_DATEFORMAT_XSLTTEXT##
                          </xsl:when>
                          <xsl:otherwise>
                            <span style="color:red">##DIFF_REPORT_DATEFORMAT_XSLTTEXT##</span>
                          </xsl:otherwise>
                        </xsl:choose>
                      </xsl:when>
                      <xsl:otherwise>
                      ##DIFF_REPORT_DATEFORMAT_XSLTTEXT##
                      </xsl:otherwise>
                    </xsl:choose>
                    </td>
                  </xsl:if>
                </xsl:for-each>
                <xsl:if test="$fp-cnt &gt; 0">
                  <td class="issuenone" colspan="{$fp-cnt}"></td>
                </xsl:if>
              </tr>
            </xsl:for-each>
          </table>
        </td>
      </tr>
    </xsl:template>

    <xsl:template match="##XML_ELEMENT_ROOT##">
        <html>
            <head>
                <link rel="stylesheet" href="##XSLT_SHEET_DIRECTORY_NAME##/##XSLT_SHEET_CSS_NAME##" />
                <title>##DIFF_REPORT_TITLE##
                  <xsl:choose>
                    <xsl:when test="@##XML_ELEMENT_ROOT_ATTRIBUTE_CHANGED##='True'">
                      &#32;(##DIFF_REPORT_DIFF_FOUND_TITLE##)
                    </xsl:when>
                    <xsl:when test="@##XML_ELEMENT_ROOT_ATTRIBUTE_CHANGED##='False'">
                      &#32;(##DIFF_REPORT_DIFF_NOT_FOUND_TITLE##)
                    </xsl:when>
                  </xsl:choose>
                </title>
                <script type="text/javascript">
                  function outliner () {
                    var oMe = event.srcElement;
                    //get child element
                    var child = document.getElementById(oMe.getAttribute("data-child"));
                    //if child element exists, expand or collapse it.
                    if (child != null)
                      child.className = (child.className == "collapsed") ? "expanded" : "collapsed";
                  }
                  function changepic() {
                    var uMe = event.srcElement;
                    uMe.src = (uMe.src.lastIndexOf("##XSLT_SHEET_IMAGE_PLUS_NAME##") != -1) ? "##XSLT_SHEET_DIRECTORY_NAME##/##XSLT_SHEET_IMAGE_MINUS_NAME##" : "##XSLT_SHEET_DIRECTORY_NAME##/##XSLT_SHEET_IMAGE_PLUS_NAME##";
                  }
                  ##DIFF_REPORT_DATEFORMAT_JS_FUNCTION##
                </script>
            </head>
            <body onclick="outliner()">
              <h1>##DIFF_REPORT_TITLE## -&#32;
              <xsl:choose>
                <xsl:when test="@##XML_ELEMENT_ROOT_ATTRIBUTE_COMMENT##">
                  <xsl:value-of select="@##XML_ELEMENT_ROOT_ATTRIBUTE_COMMENT##"/>
                </xsl:when>
                <xsl:otherwise>
                  <xsl:value-of select="##XML_ELEMENT_DIRECTORY##/@##XML_ELEMENT_ATTRIBUTE_NAME##"/>
                </xsl:otherwise>
              </xsl:choose>
              <xsl:choose>
                <xsl:when test="@##XML_ELEMENT_ROOT_ATTRIBUTE_CHANGED##='True'">
                  &#32;<span style="color:tomato">(##DIFF_REPORT_DIFF_FOUND_TITLE##)</span>
                </xsl:when>
                <xsl:when test="@##XML_ELEMENT_ROOT_ATTRIBUTE_CHANGED##='False'">
                  &#32;<span style="color:lightgreen">(##DIFF_REPORT_DIFF_NOT_FOUND_TITLE##)</span>
                </xsl:when>
              </xsl:choose>
              </h1>
              <p><span class="note">
              <xsl:if test="@##XML_ELEMENT_ROOT_ATTRIBUTE_TIME_FIRSTSCAN##">
                <b>First scan:</b><br/>
                <xsl:for-each select="@##XML_ELEMENT_ROOT_ATTRIBUTE_TIME_FIRSTSCAN##">
                <xsl:variable name="prop-name" select="name(.)"/>
                ##DIFF_REPORT_DATEFORMAT_XSLTTEXT##<br/>
                </xsl:for-each>
                <xsl:if test="@##XML_ELEMENT_ROOT_ATTRIBUTE_COMMENT##">
                  ##DIFF_REPORT_SCAN_COMMENT_TITLE##:&#32;<xsl:value-of select="@##XML_ELEMENT_ROOT_ATTRIBUTE_COMMENT##"/><br/>
                </xsl:if>
                <b>Second scan:</b><br/>
                <xsl:for-each select="@##XML_ELEMENT_ROOT_ATTRIBUTE_TIME_SECONDSCAN##">
                <xsl:variable name="prop-name" select="name(.)"/>
                ##DIFF_REPORT_DATEFORMAT_XSLTTEXT##<br/>
                </xsl:for-each>
                <xsl:if test="@##XML_ELEMENT_ROOT_ATTRIBUTE_COMMENT2##">
                  ##DIFF_REPORT_SCAN_COMMENT_TITLE##:&#32;<xsl:value-of select="@##XML_ELEMENT_ROOT_ATTRIBUTE_COMMENT2##"/><br/>
                </xsl:if>
              </xsl:if>
              <xsl:if test="not(@##XML_ELEMENT_ROOT_ATTRIBUTE_TIME_FIRSTSCAN##) and @##XML_ELEMENT_ROOT_ATTRIBUTE_STARTTIME##">
                <b>Scan:</b><br/>
                <xsl:for-each select="@##XML_ELEMENT_ROOT_ATTRIBUTE_STARTTIME##">
                <xsl:variable name="prop-name" select="name(.)"/>
                ##DIFF_REPORT_DATEFORMAT_XSLTTEXT##<br/>
                </xsl:for-each>
                <xsl:if test="@##XML_ELEMENT_ROOT_ATTRIBUTE_COMMENT##">
                  ##DIFF_REPORT_SCAN_COMMENT_TITLE##:&#32;<xsl:value-of select="@##XML_ELEMENT_ROOT_ATTRIBUTE_COMMENT##"/><br/>
                </xsl:if>
              </xsl:if>
              </span></p>
              <xsl:for-each select="##XML_ELEMENT_DIRECTORY##">
                <xsl:variable name="prop-cnt" select="count(@*)-1"/>
                <table style="width:99%;" class="infotable">
                  <tr>
                    <td class="header">##XML_ELEMENT_ATTRIBUTE_NAME##</td>
                    <td class="header">##DIFF_REPORT_ALTERED_STATUS_TITLE##</td>
                    <xsl:for-each select="@*">
                      <xsl:variable name="prop-name" select="name(.)"/>
                      <xsl:if test="not($prop-name='##XML_ELEMENT_ATTRIBUTE_NAME##')">
                        <td class="header"><xsl:value-of select="$prop-name"/></td>
                      </xsl:if>
                    </xsl:for-each>
                  </tr>
                  <xsl:apply-templates select="."/>
                  <tr style="vertical-align:top;">
                    <td class="foot">##DIFF_REPORT_SUMMARY_TITLE##</td>
                    <td class="foot">
                        <b>##DIFF_REPORT_SUMMARY_FILE_TITLE##:</b>
                        ##XML_ELEMENT_ALTERED_ATTRIBUTE_ACTION_VALUE_CHANGED##=<xsl:value-of select="count(.//##XML_ELEMENT_FILE##[##XML_ELEMENT_ALTERED##/@##XML_ELEMENT_ALTERED_ATTRIBUTE_ACTION##='##XML_ELEMENT_ALTERED_ATTRIBUTE_ACTION_VALUE_CHANGED##'])"/>,
                        ##XML_ELEMENT_ALTERED_ATTRIBUTE_ACTION_VALUE_DELETED##=<xsl:value-of select="count(.//##XML_ELEMENT_FILE##[##XML_ELEMENT_ALTERED##/@##XML_ELEMENT_ALTERED_ATTRIBUTE_ACTION##='##XML_ELEMENT_ALTERED_ATTRIBUTE_ACTION_VALUE_DELETED##'])"/>,
                        ##XML_ELEMENT_ALTERED_ATTRIBUTE_ACTION_VALUE_CREATED##=<xsl:value-of select="count(.//##XML_ELEMENT_FILE##[##XML_ELEMENT_ALTERED##/@##XML_ELEMENT_ALTERED_ATTRIBUTE_ACTION##='##XML_ELEMENT_ALTERED_ATTRIBUTE_ACTION_VALUE_CREATED##'])"/>,
                        ##XML_ELEMENT_ALTERED_ATTRIBUTE_ACTION_VALUE_RENAMED##=<xsl:value-of select="count(.//##XML_ELEMENT_FILE##[##XML_ELEMENT_ALTERED##/@##XML_ELEMENT_ALTERED_ATTRIBUTE_ACTION##='##XML_ELEMENT_ALTERED_ATTRIBUTE_ACTION_VALUE_RENAMED##'])"/><br/>
                        <b>##DIFF_REPORT_SUMMARY_DIRECTORY_TITLE##:</b>
                        ##XML_ELEMENT_ALTERED_ATTRIBUTE_ACTION_VALUE_CHANGED##=<xsl:value-of select="count(.//##XML_ELEMENT_DIRECTORY##[##XML_ELEMENT_ALTERED##/@##XML_ELEMENT_ALTERED_ATTRIBUTE_ACTION##='##XML_ELEMENT_ALTERED_ATTRIBUTE_ACTION_VALUE_CHANGED##'])+count(##XML_ELEMENT_ALTERED##[@##XML_ELEMENT_ALTERED_ATTRIBUTE_ACTION##='##XML_ELEMENT_ALTERED_ATTRIBUTE_ACTION_VALUE_CHANGED##'])"/>,
                        ##XML_ELEMENT_ALTERED_ATTRIBUTE_ACTION_VALUE_DELETED##=<xsl:value-of select="count(.//##XML_ELEMENT_DIRECTORY##[##XML_ELEMENT_ALTERED##/@##XML_ELEMENT_ALTERED_ATTRIBUTE_ACTION##='##XML_ELEMENT_ALTERED_ATTRIBUTE_ACTION_VALUE_DELETED##'])+count(##XML_ELEMENT_ALTERED##[@##XML_ELEMENT_ALTERED_ATTRIBUTE_ACTION##='##XML_ELEMENT_ALTERED_ATTRIBUTE_ACTION_VALUE_DELETED##'])"/>,
                        ##XML_ELEMENT_ALTERED_ATTRIBUTE_ACTION_VALUE_CREATED##=<xsl:value-of select="count(.//##XML_ELEMENT_DIRECTORY##[##XML_ELEMENT_ALTERED##/@##XML_ELEMENT_ALTERED_ATTRIBUTE_ACTION##='##XML_ELEMENT_ALTERED_ATTRIBUTE_ACTION_VALUE_CREATED##'])+count(##XML_ELEMENT_ALTERED##[@##XML_ELEMENT_ALTERED_ATTRIBUTE_ACTION##='##XML_ELEMENT_ALTERED_ATTRIBUTE_ACTION_VALUE_CREATED##'])"/>,
                        ##XML_ELEMENT_ALTERED_ATTRIBUTE_ACTION_VALUE_RENAMED##=<xsl:value-of select="count(.//##XML_ELEMENT_DIRECTORY##[##XML_ELEMENT_ALTERED##/@##XML_ELEMENT_ALTERED_ATTRIBUTE_ACTION##='##XML_ELEMENT_ALTERED_ATTRIBUTE_ACTION_VALUE_RENAMED##'])+count(##XML_ELEMENT_ALTERED##[@##XML_ELEMENT_ALTERED_ATTRIBUTE_ACTION##='##XML_ELEMENT_ALTERED_ATTRIBUTE_ACTION_VALUE_RENAMED##'])"/>
                    </td>
                    <td class="foot" colspan="{$prop-cnt}">&#32;</td>
                  </tr>
                </table>
              </xsl:for-each>
              <p></p>
              <table class="notetable" style="width:98%;">
                <tr>
                  <td style="white-space:nowrap;">
                    <b>##DIFF_REPORT_TITLE## Settings</b>
                  </td>
                </tr>
                <tr>
                  <td>
                    ##DIFF_REPORT_SETTING_FILEMASK_TITLE##:&#32;<xsl:value-of select="@##XML_ELEMENT_ROOT_ATTRIBUTE_FILEMASK##"/><br/>
                    ##DIFF_REPORT_SETTING_LOWERCASE_TITLE##:&#32;<xsl:value-of select="@##XML_ELEMENT_ROOT_ATTRIBUTE_LOWERCASE##"/><br/>
                    ##DIFF_REPORT_SETTING_TIMEFORMAT_TITLE##:&#32;<xsl:value-of select="@##XML_ELEMENT_ROOT_ATTRIBUTE_TIMEFORMAT##"/><br/>
                    ##DIFF_REPORT_SETTING_BADSIG_TITLE##:&#32;<xsl:value-of select="@##XML_ELEMENT_ROOT_ATTRIBUTE_BADSIG##"/><br/>
                    ##DIFF_REPORT_SETTING_EXCLUDED_COMPANY_TITLE##:&#32;<xsl:value-of select="@##XML_ELEMENT_ROOT_ATTRIBUTE_EXCLUDED_COMPANY##"/>
                  </td>
                </tr>
              </table>
            </body>
        </html>
    </xsl:template>
</xsl:stylesheet>
