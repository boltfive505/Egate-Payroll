<?xml version = "1.0" encoding = "UTF-8"?>
<!DOCTYPE html>
<html xsl:version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <head>
        <meta http-equiv="X-UA-Compatible" content="IE=11"/>
        <style>
            * {
                -moz-box-sizing: border-box; 
                -webkit-box-sizing: border-box; 
                box-sizing: border-box;
            }
            body {
                font-family: 'Gill Sans', 'Gill Sans MT', Calibri, 'Trebuchet MS', sans-serif;
                font-size: 8pt;
            }
            p,h1,h2,h3,h4 {
                margin: 0;
            }
            table {
                table-layout: fixed;
                border-collapse: collapse;
                width: 100%;
            }
            table td,
            table th {
                padding: 0;
                margin: 0;
            }

            .mb-sm {
                margin-bottom: 10px;
            }
            .mb-lg {
                margin-bottom: 20px;
            }
            .mb-xlg {
                margin-bottom: 50px;
            }

            @media only screen {
                .payslip_container {
                    border: 1px solid #000000;
                    width: 148mm;
                    height: 210mm;
                    padding: 0.25in;
                    margin-bottom: 30px;
                }
            }

            @media only print {
                .payslip_container {
                    page-break-after: always;
                }
				.payslip_container {
                    page-break-after: always;
                }
				.payslip_container:last-child {
                    page-break-after: avoid;
                }
            }

            .details_table td {
                vertical-align: top;
            }
            .header_title {
                font-weight: bold;
                font-size: 18pt;
                text-align: right;
            }
            .company_details .company_name {
                font-weight: bold;
                font-size: 16pt;
                text-align: left;
            }

            .payslip_items_table td,
            .payslip_items_table th {
                padding: 3px 5px 3px 5px;
                vertical-align: middle;
                border-top: 1px dotted #000000;
                border-bottom: 1px dotted #000000;
            }
            .payslip_items_table thead tr th:nth-child(n+2),
            .payslip_items_table tbody tr td:nth-child(n+2),
            .payslip_items_table tfoot tr td {
                text-align: right;
            }
            .payslip_items_table thead tr th:nth-child(1),
            .payslip_items_table tbody tr td:nth-child(1) {
                text-align: left;
            }
            .payslip_items_table tbody tr td:nth-child(1) {
                font-weight: bold;
            }
            .payslip_items_table thead th,
            .payslip_items_table tfoot td {
                background-color: darkgray;
            }
            .payslip_items_table tbody td:nth-last-child(1) {
                background-color: gainsboro;
            }
        </style>
    </head>
    <body>
        <xsl:for-each select="PayslipItems/Payslip">
            <div class="payslip_container">
                <table class="payslip_header details_table mb-sm">
                    <tr>
                        <td class="company_details">
                            <p class="company_name">Egate Inc.</p>
                            <p>Nanay Nora Bldg. Cavite Viejo Centennial Road, Gahak, Kawit, Cavite</p>
                            <p><strong>Contact #:</strong> 0999 999 9999</p>
                        </td>
                        <td class="header_title">SALARY PAYSLIP</td>
                    </tr>
                </table>
                <table class="payslip_details details_table mb-sm">
                    <tr>
                        <td>
                            <p><strong>Employee: </strong><xsl:value-of select="EmployeeName"/></p>
                            <!-- <p><strong>Employee ID: </strong>123-456</p> -->
                        </td>
                        <td>
                            <p><strong>Pay Period: </strong><xsl:value-of select="PayPeriodStart"/> - <xsl:value-of select="PayPeriodEnd"/></p>
                            <p><strong>Pay Date: </strong><xsl:value-of select="PayDate"/></p>
                        </td>
                    </tr>
                </table>
                <table class="earnings_table payslip_items_table mb-lg">
                    <colgroup>
                        <col/>
                        <col style="width: 80px;"/>
                        <col style="width: 80px;"/>
                        <col style="width: 100px;"/>
                    </colgroup>
                    <thead>
                        <tr>
                            <th>EARNINGS</th>
                            <th>HOURS</th>
                            <th>RATE</th>
                            <th>CURRENT</th>
                        </tr>
                    </thead>
                    <tbody>
                        <xsl:for-each select="Earnings/Earning">
                            <tr>
                                <td><xsl:value-of select="@Name"/></td>
                                <td><xsl:value-of select="Hours"/></td>
                                <td><xsl:value-of select="Rate"/></td>
                                <td><xsl:value-of select="Current"/></td>
                            </tr>
                        </xsl:for-each>
                    </tbody>
                    <tfoot>
                        <tr>
                            <td colspan="3"><strong>GROSS PAY</strong></td>
                            <td><xsl:value-of select="GrossPay"/></td>
                        </tr>
                    </tfoot>
                </table>
                <table class="deductions_table payslip_items_table mb-lg">
                    <colgroup>
                        <col/>
                        <col style="width: 100px;"/>
                    </colgroup>
                    <thead>
                        <tr>
                            <th>DEDUCTIONS</th>
                            <th>CURRENT</th>
                        </tr>
                    </thead>
                    <tbody>
                        <xsl:for-each select="Deductions/Deduction">
                        <tr>
                            <td><xsl:value-of select="@Name"/></td>
                            <td><xsl:value-of select="current()"/></td>
                        </tr>
                        </xsl:for-each>
                    </tbody>
                    <tfoot>
                        <tr>
                            <td colspan="1"><strong>TOTAL DEDUCTIONS</strong></td>
                            <td><xsl:value-of select="TotalDeductions"/></td>
                        </tr>
                    </tfoot>
                </table>
                <table class="net_pay_table payslip_items_table mb-lg">
                    <colgroup>
                        <col/>
                        <col style="width: 100px;"/>
                    </colgroup>
                    <tbody>
                        <tr>
                            <td>NET PAY</td>
                            <td><strong><xsl:value-of select="NetPay"/></strong></td>
                        </tr>
                    </tbody>
                </table>
                <div class="notes mb-xlg">
                    <p><strong>Employee Notes</strong></p>
                    <p><xsl:value-of select="EmployeeNotes"/></p>
                </div>
                <table class="signatures">
                    <tr>
                        <td>_________________________</td>
                        <td>_________________________</td>
                    </tr>
                    <tr>
                        <td>Employee's Signature</td>
						<td>Employer's Signature</td>
                    </tr>
                </table>
            </div>
        </xsl:for-each>
    </body>
</html>