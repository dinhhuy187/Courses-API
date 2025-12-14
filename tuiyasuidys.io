<mxfile host="app.diagrams.net">
    <diagram name="System Architecture" id="architecture">
        <mxGraphModel dx="1422" dy="794" grid="1" gridSize="10" guides="1" tooltips="1" connect="1" arrows="1" fold="1" page="1" pageScale="1" pageWidth="1169" pageHeight="827" math="0" shadow="0">
            <root>
                <mxCell id="0" />
                <mxCell id="1" parent="0" />

                <!-- CLIENT TIER -->
                <mxCell id="client-tier-box" value="" style="rounded=0;whiteSpace=wrap;html=1;fillColor=#dae8fc;strokeColor=#6c8ebf;verticalAlign=top;" vertex="1" parent="1">
                    <mxGeometry x="80" y="40" width="1000" height="180" as="geometry" />
                </mxCell>
                <mxCell id="client-tier-label" value="CLIENT TIER (Frontend)" style="text;html=1;strokeColor=none;fillColor=none;align=center;verticalAlign=middle;whiteSpace=wrap;rounded=0;fontSize=16;fontStyle=1" vertex="1" parent="1">
                    <mxGeometry x="80" y="50" width="200" height="30" as="geometry" />
                </mxCell>

                <!-- React Application Box -->
                <mxCell id="react-app-box" value="" style="rounded=1;whiteSpace=wrap;html=1;fillColor=#fff2cc;strokeColor=#d6b656;" vertex="1" parent="1">
                    <mxGeometry x="120" y="90" width="920" height="110" as="geometry" />
                </mxCell>
                <mxCell id="react-label" value="React Application (SPA)" style="text;html=1;strokeColor=none;fillColor=none;align=left;verticalAlign=top;whiteSpace=wrap;rounded=0;fontSize=14;fontStyle=1" vertex="1" parent="1">
                    <mxGeometry x="140" y="100" width="200" height="30" as="geometry" />
                </mxCell>

                <!-- React Components -->
                <mxCell id="components" value="Components" style="rounded=1;whiteSpace=wrap;html=1;fillColor=#e1d5e7;strokeColor=#9673a6;" vertex="1" parent="1">
                    <mxGeometry x="140" y="140" width="160" height="40" as="geometry" />
                </mxCell>
                <mxCell id="pages" value="Pages" style="rounded=1;whiteSpace=wrap;html=1;fillColor=#e1d5e7;strokeColor=#9673a6;" vertex="1" parent="1">
                    <mxGeometry x="320" y="140" width="160" height="40" as="geometry" />
                </mxCell>
                <mxCell id="services" value="Services (API calls)" style="rounded=1;whiteSpace=wrap;html=1;fillColor=#e1d5e7;strokeColor=#9673a6;" vertex="1" parent="1">
                    <mxGeometry x="500" y="140" width="160" height="40" as="geometry" />
                </mxCell>
                <mxCell id="state-mgmt" value="State Management&#xa;(Redux/Context)" style="rounded=1;whiteSpace=wrap;html=1;fillColor=#e1d5e7;strokeColor=#9673a6;" vertex="1" parent="1">
                    <mxGeometry x="680" y="140" width="160" height="40" as="geometry" />
                </mxCell>
                <mxCell id="routing" value="Routing" style="rounded=1;whiteSpace=wrap;html=1;fillColor=#e1d5e7;strokeColor=#9673a6;" vertex="1" parent="1">
                    <mxGeometry x="860" y="140" width="160" height="40" as="geometry" />
                </mxCell>

                <!-- Arrow from Client to Backend -->
                <mxCell id="arrow-client-backend" value="" style="endArrow=classic;startArrow=classic;html=1;rounded=0;strokeWidth=2;strokeColor=#000000;" edge="1" parent="1">
                    <mxGeometry width="50" height="50" relative="1" as="geometry">
                        <mxPoint x="580" y="220" as="sourcePoint" />
                        <mxPoint x="580" y="280" as="targetPoint" />
                    </mxGeometry>
                </mxCell>
                <mxCell id="arrow-label-1" value="HTTP/HTTPS (REST API)&#xa;JSON" style="text;html=1;strokeColor=none;fillColor=none;align=center;verticalAlign=middle;whiteSpace=wrap;rounded=0;fontSize=12;fontStyle=1" vertex="1" parent="1">
                    <mxGeometry x="600" y="235" width="160" height="30" as="geometry" />
                </mxCell>

                <!-- APPLICATION TIER -->
                <mxCell id="app-tier-box" value="" style="rounded=0;whiteSpace=wrap;html=1;fillColor=#d5e8d4;strokeColor=#82b366;verticalAlign=top;" vertex="1" parent="1">
                    <mxGeometry x="80" y="280" width="1000" height="420" as="geometry" />
                </mxCell>
                <mxCell id="app-tier-label" value="APPLICATION TIER (Backend)" style="text;html=1;strokeColor=none;fillColor=none;align=center;verticalAlign=middle;whiteSpace=wrap;rounded=0;fontSize=16;fontStyle=1" vertex="1" parent="1">
                    <mxGeometry x="80" y="290" width="250" height="30" as="geometry" />
                </mxCell>

                <!-- ASP.NET Core Box -->
                <mxCell id="aspnet-box" value="" style="rounded=1;whiteSpace=wrap;html=1;fillColor=#f5f5f5;strokeColor=#666666;fontColor=#333333;" vertex="1" parent="1">
                    <mxGeometry x="120" y="330" width="920" height="350" as="geometry" />
                </mxCell>
                <mxCell id="aspnet-label" value="ASP.NET Core Web API" style="text;html=1;strokeColor=none;fillColor=none;align=left;verticalAlign=top;whiteSpace=wrap;rounded=0;fontSize=14;fontStyle=1" vertex="1" parent="1">
                    <mxGeometry x="140" y="340" width="200" height="30" as="geometry" />
                </mxCell>

                <!-- Presentation Layer -->
                <mxCell id="presentation-layer" value="" style="rounded=1;whiteSpace=wrap;html=1;fillColor=#ffe6cc;strokeColor=#d79b00;" vertex="1" parent="1">
                    <mxGeometry x="160" y="380" width="840" height="70" as="geometry" />
                </mxCell>
                <mxCell id="presentation-label" value="Presentation Layer" style="text;html=1;strokeColor=none;fillColor=none;align=left;verticalAlign=top;whiteSpace=wrap;rounded=0;fontSize=13;fontStyle=1" vertex="1" parent="1">
                    <mxGeometry x="180" y="385" width="150" height="20" as="geometry" />
                </mxCell>
                <mxCell id="controllers" value="Controllers" style="rounded=1;whiteSpace=wrap;html=1;fillColor=#fff2cc;strokeColor=#d6b656;" vertex="1" parent="1">
                    <mxGeometry x="180" y="410" width="240" height="30" as="geometry" />
                </mxCell>
                <mxCell id="middleware" value="Middleware" style="rounded=1;whiteSpace=wrap;html=1;fillColor=#fff2cc;strokeColor=#d6b656;" vertex="1" parent="1">
                    <mxGeometry x="440" y="410" width="240" height="30" as="geometry" />
                </mxCell>
                <mxCell id="dtos" value="DTOs (Data Transfer Objects)" style="rounded=1;whiteSpace=wrap;html=1;fillColor=#fff2cc;strokeColor=#d6b656;" vertex="1" parent="1">
                    <mxGeometry x="700" y="410" width="260" height="30" as="geometry" />
                </mxCell>

                <!-- Business Logic Layer -->
                <mxCell id="business-layer" value="" style="rounded=1;whiteSpace=wrap;html=1;fillColor=#d5e8d4;strokeColor=#82b366;" vertex="1" parent="1">
                    <mxGeometry x="160" y="470" width="840" height="70" as="geometry" />
                </mxCell>
                <mxCell id="business-label" value="Business Logic Layer" style="text;html=1;strokeColor=none;fillColor=none;align=left;verticalAlign=top;whiteSpace=wrap;rounded=0;fontSize=13;fontStyle=1" vertex="1" parent="1">
                    <mxGeometry x="180" y="475" width="170" height="20" as="geometry" />
                </mxCell>
                <mxCell id="services-impl" value="Services (Implementation)" style="rounded=1;whiteSpace=wrap;html=1;fillColor=#e1f5e1;strokeColor=#82b366;" vertex="1" parent="1">
                    <mxGeometry x="180" y="500" width="240" height="30" as="geometry" />
                </mxCell>
                <mxCell id="iservices" value="IServices (Interfaces)" style="rounded=1;whiteSpace=wrap;html=1;fillColor=#e1f5e1;strokeColor=#82b366;" vertex="1" parent="1">
                    <mxGeometry x="440" y="500" width="240" height="30" as="geometry" />
                </mxCell>
                <mxCell id="business-rules" value="Business Rules &amp; Validation" style="rounded=1;whiteSpace=wrap;html=1;fillColor=#e1f5e1;strokeColor=#82b366;" vertex="1" parent="1">
                    <mxGeometry x="700" y="500" width="260" height="30" as="geometry" />
                </mxCell>

                <!-- Data Access Layer -->
                <mxCell id="data-layer" value="" style="rounded=1;whiteSpace=wrap;html=1;fillColor=#dae8fc;strokeColor=#6c8ebf;" vertex="1" parent="1">
                    <mxGeometry x="160" y="560" width="840" height="70" as="geometry" />
                </mxCell>
                <mxCell id="data-label" value="Data Access Layer" style="text;html=1;strokeColor=none;fillColor=none;align=left;verticalAlign=top;whiteSpace=wrap;rounded=0;fontSize=13;fontStyle=1" vertex="1" parent="1">
                    <mxGeometry x="180" y="565" width="150" height="20" as="geometry" />
                </mxCell>
                <mxCell id="entity" value="Entity (Models)" style="rounded=1;whiteSpace=wrap;html=1;fillColor=#dae8fc;strokeColor=#6c8ebf;" vertex="1" parent="1">
                    <mxGeometry x="180" y="590" width="240" height="30" as="geometry" />
                </mxCell>
                <mxCell id="dbcontext" value="DbContext (EF Core)" style="rounded=1;whiteSpace=wrap;html=1;fillColor=#dae8fc;strokeColor=#6c8ebf;" vertex="1" parent="1">
                    <mxGeometry x="440" y="590" width="240" height="30" as="geometry" />
                </mxCell>
                <mxCell id="repositories" value="Repositories (if used)" style="rounded=1;whiteSpace=wrap;html=1;fillColor=#dae8fc;strokeColor=#6c8ebf;" vertex="1" parent="1">
                    <mxGeometry x="700" y="590" width="260" height="30" as="geometry" />
                </mxCell>

                <!-- Cross-Cutting Concerns -->
                <mxCell id="cross-cutting" value="" style="rounded=1;whiteSpace=wrap;html=1;fillColor=#f8cecc;strokeColor=#b85450;" vertex="1" parent="1">
                    <mxGeometry x="160" y="640" width="840" height="30" as="geometry" />
                </mxCell>
                <mxCell id="cross-label" value="Cross-Cutting Concerns:  Helper  |  Authentication/Authorization  |  Logging  |  Exception Handling" style="text;html=1;strokeColor=none;fillColor=none;align=center;verticalAlign=middle;whiteSpace=wrap;rounded=0;fontSize=12;fontStyle=1" vertex="1" parent="1">
                    <mxGeometry x="180" y="645" width="800" height="20" as="geometry" />
                </mxCell>

                <!-- Arrow from Backend to Database -->
                <mxCell id="arrow-backend-db" value="" style="endArrow=classic;startArrow=classic;html=1;rounded=0;strokeWidth=2;strokeColor=#000000;" edge="1" parent="1">
                    <mxGeometry width="50" height="50" relative="1" as="geometry">
                        <mxPoint x="580" y="700" as="sourcePoint" />
                        <mxPoint x="580" y="760" as="targetPoint" />
                    </mxGeometry>
                </mxCell>
                <mxCell id="arrow-label-2" value="ADO.NET / EF Core" style="text;html=1;strokeColor=none;fillColor=none;align=center;verticalAlign=middle;whiteSpace=wrap;rounded=0;fontSize=12;fontStyle=1" vertex="1" parent="1">
                    <mxGeometry x="600" y="715" width="150" height="30" as="geometry" />
                </mxCell>

                <!-- DATA TIER -->
                <mxCell id="data-tier-box" value="" style="rounded=0;whiteSpace=wrap;html=1;fillColor=#f5f5f5;strokeColor=#666666;fontColor=#333333;verticalAlign=top;" vertex="1" parent="1">
                    <mxGeometry x="80" y="760" width="1000" height="180" as="geometry" />
                </mxCell>
                <mxCell id="data-tier-label" value="DATA TIER (Database)" style="text;html=1;strokeColor=none;fillColor=none;align=center;verticalAlign=middle;whiteSpace=wrap;rounded=0;fontSize=16;fontStyle=1" vertex="1" parent="1">
                    <mxGeometry x="80" y="770" width="200" height="30" as="geometry" />
                </mxCell>

                <!-- PostgreSQL Box -->
                <mxCell id="postgres-box" value="" style="rounded=1;whiteSpace=wrap;html=1;fillColor=#e1d5e7;strokeColor=#9673a6;" vertex="1" parent="1">
                    <mxGeometry x="120" y="810" width="920" height="110" as="geometry" />
                </mxCell>
                <mxCell id="postgres-label" value="PostgreSQL Database" style="text;html=1;strokeColor=none;fillColor=none;align=left;verticalAlign=top;whiteSpace=wrap;rounded=0;fontSize=14;fontStyle=1" vertex="1" parent="1">
                    <mxGeometry x="140" y="820" width="200" height="30" as="geometry" />
                </mxCell>

                <!-- Database Components -->
                <mxCell id="tables" value="Tables" style="rounded=1;whiteSpace=wrap;html=1;fillColor=#f8cecc;strokeColor=#b85450;" vertex="1" parent="1">
                    <mxGeometry x="140" y="860" width="200" height="40" as="geometry" />
                </mxCell>
                <mxCell id="procedures" value="Stored Procedures/&#xa;Functions" style="rounded=1;whiteSpace=wrap;html=1;fillColor=#f8cecc;strokeColor=#b85450;" vertex="1" parent="1">
                    <mxGeometry x="360" y="860" width="200" height="40" as="geometry" />
                </mxCell>
                <mxCell id="triggers" value="Triggers" style="rounded=1;whiteSpace=wrap;html=1;fillColor=#f8cecc;strokeColor=#b85450;" vertex="1" parent="1">
                    <mxGeometry x="580" y="860" width="200" height="40" as="geometry" />
                </mxCell>
                <mxCell id="indexes" value="Indexes" style="rounded=1;whiteSpace=wrap;html=1;fillColor=#f8cecc;strokeColor=#b85450;" vertex="1" parent="1">
                    <mxGeometry x="800" y="860" width="200" height="40" as="geometry" />
                </mxCell>

            </root>
        </mxGraphModel>
    </diagram>
</mxfile>