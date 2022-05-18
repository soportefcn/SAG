/// <reference path="funciones.js" />
$(document).ready(function () {
    if ($("#BorrarLinea").val() != undefined) {
        $("#BorrarLinea").hide();
    }
    if ($("#BorrarLineaR").val() != undefined) {
        $("#BorrarLineaR").hide();
    }
    var mesesCorto = ["Ene.", "Feb.", "Mar.", "Abr.", "May.", "Jun.", "Jul.", "Ago.", "Sep.", "Oct.", "Nov.", "Dic."];

    $("th").attr("nowrap", "nowrap");
    //$("td").attr("nowrap", "nowrap");

    // Muestra y oculta los menús
    $('ul li:has(ul)').hover(
		    function (e) {
		        $(this).find('ul').show();
		    },
		    function (e) {
		        $(this).find('ul').hide();
		    });

    // Alertas
    /*
    for (var i = 0; i < 5; i++) {
    $("#alertas").fadeIn(100).delay(200).fadeOut(100);
    if (i == 4)
    $("#alertas").fadeIn(100).delay(200);
    };
    */
    $(".Entradabodega").click(function () {

        window.parent.document.getElementById("ID").value = $(this).attr("MovimientosBodegaID");
        window.parent.document.getElementById("DetalleEgresoID").value = $(this).attr("DetalleEgresoID");
        window.parent.document.getElementById("CuentaID").value = $(this).attr("CuentaID");
        window.parent.document.getElementById("NroDocumento").value = $(this).attr("Negreso");
        window.parent.document.getElementById("cuentad").value = $(this).attr("cuentad");
        window.parent.document.getElementById("ndoc").value = $(this).attr("ndocumento");
        window.parent.document.getElementById("vdoc").value = $(this).attr("valordoc");
        window.parent.document.getElementById("Entrada").value = $(this).attr("cantidad");
        window.parent.document.getElementById("Observaciones").value = $(this).attr("observacion");
       // window.parent.document.getElementById("ArticuloID").value = $(this).attr("articuloid");
        window.parent.document.getElementById("ArticuloID").value = $(this).attr("articuloid");
        window.parent.document.getElementById("ArticuloID").value = $(this).attr("nombrearticulo");
        // window.parent.document.getElementById("Entrada").value = $(this).attr("Cantidad");
        //  ndoc window.parent.document.getElementById("Salida").value = $(this).attr("Cantidad");  
    });
    $(".Salidabodega").click(function () {
        window.parent.document.getElementById("ID").value = $(this).attr("MovimientosBodegaID");
        window.parent.document.getElementById("DetalleEgresoID").value = $(this).attr("DetalleEgresoID");
        window.parent.document.getElementById("CuentaID").value = $(this).attr("CuentaID");
        window.parent.document.getElementById("NroDocumento").value = $(this).attr("Negreso");
        window.parent.document.getElementById("cuentad").value = $(this).attr("cuentad");
        window.parent.document.getElementById("ndoc").value = $(this).attr("ndocumento");
        window.parent.document.getElementById("vdoc").value = $(this).attr("valordoc");
        window.parent.document.getElementById("Salida").value = $(this).attr("cantidad");
        window.parent.document.getElementById("Observaciones").value = $(this).attr("observacion");
        window.parent.document.getElementById("ArticuloIDp").value = $(this).attr("articuloid");
        // window.parent.document.getElementById("Entrada").value = $(this).attr("Cantidad");
        //  ndoc window.parent.document.getElementById("Salida").value = $(this).attr("Cantidad");  
    });
    $("#Rut").Rut({
        digito_verificador: "#DVBuscar",
        format: false,
        on_error: function () {
            $("#Rut").addClass("input-validation-error");
            $("#DVBuscar").addClass("input-validation-error");
            $("#Rut").focus();
            $(".linkDetalle").attr("disabled", true);
            $('body').css('opacity','0.25');
            alert("El rut ingresado no es válido");
            $('body').css('opacity', '1');
            $("#DVBuscar").val("");
        },
        on_success: function () {
            $("#Rut").removeClass("input-validation-error");
            $("#DVBuscar").removeClass("input-validation-error");
            $(".linkDetalle").removeAttr("disabled");
        }
    });

    $("#Rut").Rut({
        digito_verificador: "#DVBuscar",
        format: false,
        on_error: function () {
            $("#Rut").addClass("input-validation-error");
            $("#DV").addClass("input-validation-error");
            $("#Rut").focus();
            $("#submit").attr("disabled", true);
            $('body').css('opacity','0.25');
            alert("El rut ingresado no es válido");
            $('body').css('opacity', '1');
            $("#DVBuscar").val("");
        },
        on_success: function () {
            $("#Rut").removeClass("input-validation-error");
            $("#DV").removeClass("input-validation-error");
            $("#submit").removeAttr("disabled");
        }
    });

    // Calendario
    $('.calendario_semana').datepicker({
        altFormat: "dd-mm-yy",
        dateFormat: "dd-mm-yy",
        showOtherMonths: true,
        selectOtherMonths: true,
        yearRange:"-90:+1",
        onSelect: function (dateText, inst) {
            var date = $(this).datepicker('getDate');
            startDate = new Date(date.getFullYear(), date.getMonth(), date.getDate() - date.getDay() + 1);
            endDate = new Date(date.getFullYear(), date.getMonth(), date.getDate() - date.getDay() + 5);
            var dateFormat = inst.settings.dateFormat || $.datepicker._defaults.dateFormat;
            var inicio = $.datepicker.formatDate(dateFormat, startDate, inst.settings);
            var fin = $.datepicker.formatDate(dateFormat, endDate, inst.settings);
            var actual = $.datepicker.formatDate(dateFormat, date, inst.settings);
            //$('#startDate').text($.datepicker.formatDate(dateFormat, startDate, inst.settings));
            //$('#endDate').text($.datepicker.formatDate(dateFormat, endDate, inst.settings));

            var current = window.location.pathname;
            var querystring = "?i=" + inicio + "&f=" + fin + "&s=" + actual;
            window.location.href = current + querystring;
            //alert($.datepicker.formatDate(dateFormat, startDate, inst.settings) + " " + $.datepicker.formatDate(dateFormat, endDate, inst.settings));
            //selectCurrentWeek();
        }
    });

    //Calendario mantenedores
    $('.calendario').datepicker({
        altFormat: "dd-mm-yy",
        dateFormat: "dd-mm-yy",
        changeMonth: true,
        changeYear: true,
        minDate: "-100y",
        yearRange:"-90:+6",
    });

    //Calendario movimientos
    $('.calendario_mov').datepicker({
        altFormat: "dd-mm-yy",
        dateFormat: "dd-mm-yy",
        changeMonth: false,
        changeYear: false//,
        //maxDate: "+15d",
        //minDate: "-5y"
    });


    $('.calendario_mov').removeAttr("readonly");
    $('.calendario').removeAttr("readonly");

    $('.calendario_mov').attr("maxlength", "10");
    $('.calendario').attr("maxlength", "10");






    // Selecciona comuna
    $('#RegionID').change(function () {
        $.get('/SAG_5/Comuna/Comunas/' + $(this).val(), function (response) {
            var comunas = $.parseJSON(response);
            var ddlComunas = $('#ComunaID');
            $('#ComunaID > option').remove();
            if (comunas.length == 0) {
                ddlComunas.append($("<option />").val("1").text("Sin información"));
            } else {
                ddlComunas.append($("<option />").val("1").text("Seleccione comuna"));
                for (i = 0; i < comunas.length; i++) {
                    ddlComunas.append($("<option />").val(comunas[i].Value).text(comunas[i].Text));
                }
            }
        });
    });

    //
    if ($('#RegionID').val() != "" && $('#RegionID').val() != undefined) {
        var comuna = $('#RegionID').attr("comuna");
        $.get('/SAG_5/Comuna/Comunas/' + $('#RegionID').val(), function (response) {
            var comunas = $.parseJSON(response);
            var ddlComunas = $('#ComunaID');
            $('#ComunaID > option').remove();
            if (comunas.length == 0) {
                ddlComunas.append($("<option />").val("1").text("Sin información").attr("selected", "selected"));
            } else {
                ddlComunas.append($("<option />").val("1").text("Seleccione comuna"));
                for (i = 0; i < comunas.length; i++) {
                    if (comuna == comunas[i].Value) {
                        ddlComunas.append($("<option selected=\"selected\" />").val(comunas[i].Value).text(comunas[i].Text));
                    } else {
                        ddlComunas.append($("<option />").val(comunas[i].Value).text(comunas[i].Text));
                    }
                }
            }
        });
    }

    $("#ComprobanteEgresoID").change(function () {
        $.get('/SAG_5/Reintegros/Egreso/' + $('#ComprobanteEgresoID').val(), function (response) {
            var egreso = $.parseJSON(response);
            $('#fechaEgreso').val(egreso[0].Fecha);
        });
    });

    

    $("#BuscarComprobanteEgresoID").click(function () {
        $("#ComprobanteEgreso").click();
    });

    $('#Interes').change(function () {
        if ($('#Monto').val() != "" && $('#Interes').val() != "") {
            var interesVal = $('#Interes').val();
            interesVal = interesVal.replace(",", ".");
            var montoOriginal = new Number($('#Monto').val());

            var interes = new Number(interesVal);
            var intereses = montoOriginal * (interes / 100);
            var montoFinal = montoOriginal + intereses;

            //$('#Interes').val(interesVal.replace(".", ","));
            $('#Intereses').val(intereses);
            $('#MontoFinal').val(montoFinal);

            $('#Interes').val($('#Interes').val().replace(".", ","));
            $('#Intereses').val($('#Intereses').val().replace(".", ","));
            $('#MontoFinal').val($('#MontoFinal').val().replace(".", ","));
        }
    });
    

    /*
    $("#Bruto").change(function () {
        if ($('#Bruto').val() != "") {
            var bruto = new Number($('#Bruto').val());
           
            var porce=10.75;
            var retencion = bruto * porce / 100;
       
            
         
            var checkText = $("#Porcentaje").val();;
            alert('jquery catch select text' + checkText);
            alert($(".Porcentaje").find("option:selected").text() + ' clicked!');
            var neto = bruto - retencion;
            $('#Retencion').val(Math.round(retencion));
            $('#Neto').val(Math.round(neto));
        }
    });
                */


    $("#Bruto").change(function () {
        if ($('#Bruto').val() != "") {
            var bruto = new Number($('#Bruto').val());
     
            $('#VPorce').val($('#VPorce').val().replace(",", "."));
            var porce = new Number($('#VPorce').val());
            
            var retencion = bruto * porce / 100;

            var neto = bruto - retencion;
            $('#Retencion').val(Math.round(retencion));
            $('#Neto').val(Math.round(neto));
        }
    });
    
    $("#Neto").change(function () {
        if ($('#Neto').val() != "") {
            var neto = new Number($('#Neto').val());
            var porcentaje = new Number($('#VPorce').val());
            
            var bruto = new Number(neto / (1 - (porce / 100)));
            var retencion = bruto - neto;
            $('#Retencion').val(Math.round(retencion));
            $('#Bruto').val(Math.round(bruto));
        }
    });
    
    if ($.validator != undefined) {
        $.validator.methods.number = function (value, element) {
            return parseFloat(value).toString() !== "NaN";
        }
    }


    $(".tipobeneficiario #tipoBeneficiario").change(function () {

        $(".tipobeneficiario #PersonaID").val("0");
        $(".tipobeneficiario #ProveedorID").val("0");
        $(".tipobeneficiario #Rut").val("");
        $(".tipobeneficiario #DVBuscar").val("");
        $(".tipobeneficiario #Beneficiario").val("");

        if ($(this).val() == "3") {
            $(".tipobeneficiario #Beneficiario").removeAttr("readonly");
        } else {
            $(".tipobeneficiario #Beneficiario").attr("readonly", "readonly");
        }
        
        $(".tipobeneficiario #Rut").focus();
    });


    if ($(".tipobeneficiario #tipoBeneficiario").val() != "" && $(".tipobeneficiario #tipoBeneficiario").val() != undefined) {
        if ($(".tipobeneficiario #tipoBeneficiario").val() == "1") {
            $(".tipobeneficiario #PersonaID").show();
            $(".tipobeneficiario #ProveedorID").hide();
            $(".tipobeneficiario #Otro").hide();
        } else if ($(".tipobeneficiario #tipoBeneficiario").val() == "2") {
            $(".tipobeneficiario #PersonaID").hide();
            $(".tipobeneficiario #ProveedorID").show();
            $(".tipobeneficiario #Otro").hide();
        } else {
            $(".tipobeneficiario #PersonaID").hide();
            $(".tipobeneficiario #ProveedorID").hide();
            $(".tipobeneficiario #Otro").show();
        }
    }


    /*
    Funciones para EGRESO
    */

    // Al hacer click en el monto del egreso se le indica al usuario que ese monto se completa automaticamente
    $("#Monto_Egresos").click(function(){
        $('body').css('opacity','0.25');
        alert("Este monto se calcula automáticamente con la suma del detalle.");
        $('body').css('opacity','1');
        return false;
    });

    $("#iframeDetalle").hide();
    var montoEgreso = 0;
    $("#linkGeneral").click(function () {
        $(".egreso #general").show();
        $(".egreso #detalle").hide();
        return false;
    });

    $(".linkDetalle").click(function () {
        if ($(this).attr("editar") != "" && $(this).attr("editar") != undefined) {
            $("#iframeDetalle").show();
        }
        $(".egreso #general").hide();
        $(".egreso #detalle").show();
        return false;
    });

    $(".egreso #detalle").hide();
    $(".egreso #DocumentoIDD").change(function () {
       
        if ($(this).val() == 3) {
            var tipoBeneficiario = $(".tipobeneficiario #tipoBeneficiario").val();
            var personalID = 0;
            var proveedorID = 0;
            var rut = "", dv = "", beneficiario = "";
            if (tipoBeneficiario == "1" && $(".tipobeneficiario #PersonaID").val() != "") {
                personalID = $(".tipobeneficiario #PersonaID").val();
            } else if (tipoBeneficiario == "2" && $(".tipobeneficiario #ProveedorID").val() != "") {
                proveedorID = $(".tipobeneficiario #ProveedorID").val();
            } else if (tipoBeneficiario == "3" && $(".tipobeneficiario #Rut").val() != "" && $(".tipobeneficiario #DVBuscar").val() != "" && $(".tipobeneficiario #Beneficiario").val() != "") {
                rut = $(".tipobeneficiario #Rut").val();
                dv = $(".tipobeneficiario #DVBuscar").val();
                beneficiario = $(".tipobeneficiario #Beneficiario").val();
            }
            if (personalID != 0) {
                window.open("/SAG_5/BoletasHonorarios/IngresarPopUp/?personalID=" + personalID, 'SeleccionBoletaHonorarios', 'width=800,height=600');
            } else if (proveedorID != 0) {
                window.open("/SAG_5/BoletasHonorarios/IngresarPopUp/?proveedorID=" + proveedorID, 'SeleccionBoletaHonorarios', 'width=800,height=600');
            } else {
                window.open("/SAG_5/BoletasHonorarios/IngresarPopUp/?rut=" + rut + "&dv=" + dv + "&beneficiario=" + beneficiario, 'SeleccionBoletaHonorarios', 'width=800,height=600');
            }

            $(".egreso #NComprobanteDP").attr("disabled", true);
            $(".egreso #NDocumento").attr("readonly", "readonly");
            $(".egreso #FechaEmision").attr("readonly", "readonly");
            $(".egreso #FechaVencimiento").attr("disabled", true);
            //$(".egreso #Monto").attr("readonly", "readonly");
           // $(".egreso #DocumentoID").val("3");
            $(".egreso #Origen").val("hs");
        }
    }); 

    $(".egreso #Origen").change(function () {
        $(".egreso #NComprobanteDP").removeAttr("disabled");
        $(".egreso #NDocumento").removeAttr("disabled");
        $(".egreso #FechaEmision").removeAttr("disabled");
        $(".egreso #FechaVencimiento").removeAttr("disabled");
        $(".egreso #CuentaID").removeAttr("disabled");
        $(".egreso #Glosa").removeAttr("disabled");
        $(".egreso #Monto").removeAttr("disabled");
        $(".egreso #NDocumento").removeAttr("readonly");
        $(".egreso #Monto").removeAttr("readonly");

        var tipoBeneficiario = $(".tipobeneficiario #tipoBeneficiario").val();
        var personalID = 0;
        var proveedorID = 0;
        var rut = "", dv = "", beneficiario = "";
        if (tipoBeneficiario == "1" && $(".tipobeneficiario #PersonaID").val() != "") {
            personalID = $(".tipobeneficiario #PersonaID").val();
        } else if (tipoBeneficiario == "2" && $(".tipobeneficiario #ProveedorID").val() != "") {
            proveedorID = $(".tipobeneficiario #ProveedorID").val();
        } else if (tipoBeneficiario == "3" && $(".tipobeneficiario #Rut").val() != "" && $(".tipobeneficiario #DVBuscar").val() != "" && $(".tipobeneficiario #Beneficiario").val() != "") {
            rut = $(".tipobeneficiario #Rut").val();
            dv = $(".tipobeneficiario #DVBuscar").val();
            beneficiario = $(".tipobeneficiario #Beneficiario").val();
        } else {
            if ($(this).val() != "oo") {
                $('body').css('opacity','0.25');
                alert("Para ingresar el detalle debe seleleccionar el beneficiario del egreso.");
                $('body').css('opacity','1');
                $(".egreso #general").show();
                $(this).prop('selectedIndex', 0);
                $(".egreso #detalle").hide();
                $(".tipobeneficiario #tipoBeneficiario").focus();
                return false;
            }
        }

        if ($(this).val() == "ff") {
            /****
            Deshabilitamos campos que no se utilizan
            *****/
            $(".egreso #NComprobanteDP").attr("disabled", true);
            $(".egreso #NDocumento").attr("disabled", true);
            $(".egreso #FechaEmision").attr("disabled", true);
            $(".egreso #FechaVencimiento").attr("disabled", true);
            $(".egreso #CuentaID").attr("disabled", true);
            $(".egreso #Glosa").attr("disabled", true);
            $(".egreso #Monto").attr("disabled", true);
            $(".egreso #DocumentoID").attr("disabled", true);

            /* Desplegamos los fondos fijos activos */
            window.open("/SAG_5/Egresos/FondoFijo", 'SeleccionCC', 'width=800,height=400');
            
            /*

            if (confirm("Haga click en Aceptar para agregar automáticamente el Fondo Fijo al detalle del egreso.")) {
                $(".egreso #GuardarLinea").click();
            } else {
                $(".egreso #Origen").val("0");
                $(".egreso #Origen").change();
            }
            */
            /**********/
        } else if ($(this).val() == "dp") {
            if (personalID != 0) {
                window.open("/SAG_5/DeudasPendientes/ListadoEgreso/?personalID=" + personalID, 'SeleccionDP', 'width=800,height=400');
            } else if (proveedorID != 0) {
                window.open("/SAG_5/DeudasPendientes/ListadoEgreso/?proveedorID=" + proveedorID, 'SeleccionDP', 'width=800,height=400');
            } else {
                window.open("/SAG_5/DeudasPendientes/ListadoEgreso/", 'SeleccionDP', 'width=300,height=800');
            }
            $(".egreso #NComprobanteDP").attr("readonly", "readonly");
            $(".egreso #NDocumento").attr("readonly", "readonly");
            $(".egreso #FechaEmision").attr("readonly", "readonly");
            $(".egreso #FechaVencimiento").attr("readonly", "readonly");
            $(".egreso #DocumentoID").attr("readonly", "readonly");
        } else if ($(this).val() == "hs") {
            if (personalID != 0) {
                window.open("/SAG_5/BoletasHonorarios/IngresarPopUp/?personalID=" + personalID, 'SeleccionBoletaHonorarios', 'width=800,height=600');
            } else if (proveedorID != 0) {
                window.open("/SAG_5/BoletasHonorarios/IngresarPopUp/?proveedorID=" + proveedorID, 'SeleccionBoletaHonorarios', 'width=800,height=600');
            } else {
                window.open("/SAG_5/BoletasHonorarios/IngresarPopUp/?rut=" + rut + "&dv=" + dv + "&beneficiario=" + beneficiario, 'SeleccionBoletaHonorarios', 'width=800,height=600');
            }

            $(".egreso #NComprobanteDP").attr("disabled", true);
            $(".egreso #NDocumento").attr("readonly", "readonly");
            $(".egreso #FechaEmision").attr("readonly", "readonly");
            $(".egreso #FechaVencimiento").attr("disabled", true);
            //$(".egreso #Monto").attr("readonly", "readonly");
            $(".egreso #DocumentoID").val("3");
        } else {
            $(".egreso #NComprobanteDP").attr("disabled", true);
            $(".egreso #NComprobanteDP").attr("readonly", true);
            $(".egreso #DocumentoID").removeAttr("readonly");
            $(".egreso #DocumentoID").removeAttr("disabled");
        }
    });

    $(".egreso #Origen").change();

    /*
    $(".egreso #FondoFijoGrupoID").change(function(){
        $(".egreso #GuardarLinea").click();
    });
    */

    $(".egreso #GuardarLinea").click(function () {
     
       
        if ($("#CuentaID").val() == "" && $(".egreso #Origen").val() != "ff") {
            $('body').css('opacity','0.25');
            alert("Debe seleccionar una cuenta para guardar el detalle.");
            $('body').css('opacity','1');
            $("#CuentaID").focus();
            return false;
        }

        var monto = new Number($("#Monto").val());
        var montoEgreso = new Number($("#Monto_Egresos").val());

        var montoTotal = monto + montoEgreso;
 
        
        if ($(".egreso #Origen").val() == "oo" || $(".egreso #Origen").val() == "dp" || $(".egreso #Origen").val() == "hs") {
            $.post("/SAG_5/Egresos/GuardarLinea", $("#detalle form").serialize(),
            function (data) {
                if (data == "OK") {
                    // Si el monto fue guardado leemos utilidad para sumar detalles del egreso
                    $.get('/SAG_5/Data/SumaDetalleEgreso', function(data) {
                        $('#Monto_Egresos').val(data);
                        $('body').css('opacity','0.25');
                        alert("Línea guardada con exito!");
                        $('body').css('opacity','1');
                        $(".egreso #NComprobanteDP").val("");
                        $(".egreso #DocumentoIDD").val("");
                        $(".egreso #NDocumentoD").val("");
                        $(".egreso #Monto").val("");
                        $(".egreso #CuentaID").val("");
                        $(".egreso #Glosa").val("");
                        $(".egreso #Origen").val("");
                        $(".egreso #DeudaPendienteID").val("");
                        $(".egreso #BoletaHonorarioID").val("");
                    });
                } else {
                    $('body').css('opacity','0.25');
                    alert(data);
                    $('body').css('opacity','1');
                    return false;
                }
            })
            .success(function () { })
            .error(function () { })
            .complete(function () { });
        } else if ($(".egreso #Origen").val() == "ff") {
            //alert($("#FondoFijoGrupoID").val());
            var fondoFijoGrupoID = $("#FondoFijoGrupoID").val();
            /*if ($(".egreso #DocumentoID").val() == "") {
            alert("Debe seleccionar un tipo de documento.");
            $(".egreso #DocumentoID").focus();
            return false;
            } else {*/
            //$.get('/SAG_5/FondoFijo/GenerarEgreso/?DocumentoID=' + $(".egreso #DocumentoID").val(), function (response) {
            $.get('/SAG_5/CajaChica/GenerarEgreso/?fondoFijoGrupoID=' + fondoFijoGrupoID, function (response) {
                $('body').css('opacity','0.25');
                if (response == "-1") {
                    alert("Ocurrió un error al agregar el detalle del fondo fijo.");
                    return false;
                } else if (response == "0") {
                    alert("No hay fondo fijo disponible para generar el egreso.");
                    return false;
                } else {
                    alert("Fondo fijo agregado al detalle del egreso con exito!");
                    montoEgreso = new Number(response);
                    $("#Monto_Egresos").val(montoEgreso);
                    $(".egreso #NComprobanteDP").val("");
                    $(".egreso #DocumentoIDD").val("");
                    $(".egreso #NDocumentoD").val("");
                    $(".egreso #Monto").val("");
                    $(".egreso #CuentaID").val("");
                    $(".egreso #Glosa").val("");
                    $(".egreso #Origen").val("");
                    $(".egreso #DeudaPendienteID").val("");
                    $(".egreso #BoletaHonorarioID").val("");
                }
                $('body').css('opacity','1');
            });
            //}
        }

        if ($(".egreso #DetalleEgresoID") != undefined && $(".egreso #DetalleEgresoID").val() != "" && $(".egreso #DetalleEgresoID") != null && $(".egreso #DetalleEgresoID").val() != "0") {
            $(".egreso #DetalleEgresoID").val("");
        } else {
            $(".egreso #DetalleEgresoIndex").val("");
        }

        var currSrc = $("#iframeDetalle").attr("src");
        $("#iframeDetalle").attr("src", "");
        $("#iframeDetalle").attr("src", currSrc);
        $("#iframeDetalle").show();

        $("#iframeDetalle").attr("src", function ( i, val ) { return val; });

        try {
            $('#iframeDetalle').contentWindow.location.reload(true);
            $('#iframeDetalle').contentWindow.location.reload(true);
            $('#iframeDetalle').contentWindow.location.reload(true);
            $('#iframeDetalle').contentWindow.location.reload(true);
            $('#iframeDetalle').contentWindow.location.reload(true);
            $('#iframeDetalle').contentWindow.location.reload(true);
        } catch (err) { }
        //$("#FondoFijoGrupoID").val("");
    });

    $(".egreso #BorrarLinea").click(function () {
        var monto = new Number($("#Monto").val());
        var montoEgreso = new Number($("#Monto_Egresos").val());
        // Guardamos linea del formulario si opcion es otro
        if ($(".egreso #Origen").val() == "oo" || $(".egreso #Origen").val() == "dp" || $(".egreso #Origen").val() == "hs") {
            $.post("/SAG_5/Egresos/BorrarLinea", $("#detalle form").serialize(),
            function (data) {
                if (data == "OK") {
                    $.get('/SAG_5/Data/SumaDetalleEgreso', function(data) {
                        $('#Monto_Egresos').val(data);
                        $('body').css('opacity','0.25');
                        alert("Detalle borrada con exito!");
                        $('body').css('opacity','1');
                    });
                } else {
                    $('body').css('opacity','0.25');
                    alert("Ocurrió un error al borrar el detalle.");
                    $('body').css('opacity','1');
                    return false;
                }
            })
            .success(function () { })
            .error(function () { })
            .complete(function () { });
        } else if ($(".egreso #Origen").val() == "ff") {
           
        }

        if ($(".egreso #DetalleEgresoID") != undefined && $(".egreso #DetalleEgresoID").val() != "" && $(".egreso #DetalleEgresoID") != null && $(".egreso #DetalleEgresoID").val() != "0") {
            $(".egreso #DetalleEgresoID").val("");
        } else {
            $(".egreso #DetalleEgresoIndex").val("");
        }

        var currSrc = $("#iframeDetalle").attr("src");
        $("#iframeDetalle").attr("src", "");
        $("#iframeDetalle").attr("src", currSrc);
        $("#iframeDetalle").show();

        $("#iframeDetalle").attr("src", "");
        $("#iframeDetalle").attr("src", currSrc);
        $("#iframeDetalle").show();

        $(".egreso #NComprobanteDP").val("");
        $(".egreso #DocumentoID").val("");
        $(".egreso #NDocumento").val("");
        $(".egreso #Monto").val("");
        $(".egreso #CuentaID").val("");
        $(".egreso #Glosa").val("");
        $(".egreso #Origen").val("");
        $(".egreso #DeudaPendienteID").val("");
        $(".egreso #BoletaHonorarioID").val("");
    });


    /**************  Salida ****/
   

    // Funciones para Reintegro
    $("#ComprobanteEgreso").click(function () {
        window.open("/SAG_5/Reintegros/ListadoDetalles/", 'Reintegros', 'width=1100,height=550');
        return false;
    });

    $("#ComprobanteEgresoAlta").click(function () {
        window.open("/SAG_5/Inventario/ListadoDetalles/", 'Inventario', 'width=800,height=400');
        return false;
    });

    $("#ComprobanteEgresoAltaLupa").click(function () {
        $("#ComprobanteEgresoAlta").click();
    });

    // detalle Reintegro

    // Rendicion de cuentas
    $("#GenerarRendicion").click(function () {
        $('body').css('opacity','0.25');

        var nroComprobantes = $("#nroComprobantes").val();
        var nroConciliados = $("#nroConciliados").val();

        if (nroComprobantes == "0") {
            if (!confirm("El periodo actual no contiene ningún comprobante, está seguro que desea continuar y generar la rendición?")) {
                $('body').css('opacity', '1');
                return false;
            }
        }

        if (nroConciliados == "0") {
            if (!confirm("El periodo actual no contiene ningún comprobante conciliado, está seguro que desea continuar y generar la rendición?")) {
                $('body').css('opacity', '1');
                return false;
            }
        }

        if ($("#Indemnizacion").val() == "0" && $("#IndemnizacionReq").val() == "true") {
            alert("Debe ingresar el monto de Provisión para Indemnización devengada a la fecha, si no lo conoce consulte a la Auditora.");
            $("#Indemnizacion").focus();
            $('body').css('opacity','1');
            return false;
        }
        
        if ($("#Intervenciones").val() == "") {
            alert("Para generar la Rendición de Cuentas debe ingresar el número de Intervenciones para el período.");
            $("#Intervenciones").focus();
            $('body').css('opacity','1');
            return false;
        }

        if (confirm("Al generar la Rendición de Cuentas cerrará el período actual y no podrá realizar cambios en él. Realmente desea generar la Rendición de Cuentas?")) {
            if (confirm("Para generara la Rendición debe tener definido el convenio del proyecto!")) {
                $('body').css('opacity','1');
                return true;
            }
        }


        $('body').css('opacity','1');
        return false;
    });


    // Movimientos de bodega
    $("#BodegaPeriodo").click(function () {
        var periodo = $("#PeriodoAnio").val();
        var mes = $("#PeriodoMes").val();

        var currSrc = "/SAG_5/MovimientosBodega/ListarMovimientos?Periodo=" + periodo + "&Mes=" + mes;
        $("#iframeBodega").attr("src", currSrc);
        $("#iframeBodega").show();
    });

    $("#ImprimirBodega").click(function () {
        var periodo = $("#PeriodoAnio").val();
        var mes = $("#PeriodoMes").val();
        window.open("/SAG_5/MovimientosBodega/Imprimir?Periodo=" + periodo + "&Mes=" + mes, '', '');
    });

    $("#ExcelBodega").click(function () {
        var periodo = $("#PeriodoAnio").val();
        var mes = $("#PeriodoMes").val();
        window.open("/SAG_5/MovimientosBodega/Excel?Periodo=" + periodo + "&Mes=" + mes, '', '');
    });

    $("#ImprimirSaldoBodega").click(function () {
        var periodo = $("#Periodo").val();
        var mes = $("#Mes").val();
        window.open("/SAG_5/Bodega/Imprimir?Periodo=" + periodo + "&Mes=" + mes, '', '');
    });

    $("#ExcelSaldoBodega").click(function () {
        var periodo = $("#Periodo").val();
        var mes = $("#Mes").val();
        window.open("/SAG_5/Bodega/Excel?Periodo=" + periodo + "&Mes=" + mes, '', '');
    });

    $("#BodegaMovimiento").click(function () {
        var periodo = $("#PeriodoAnio").val();
        var mes = $("#PeriodoMes").val();

        // Revisar campos
        if ($("input[name='tipo_movimiento']:checked").val() == "E") {
            if ($("#NroDocumento").val() == "") {
                alert("Debe ingresar número de documento.");
                $("#NroDocumento").focus();
                return false;
            }
        }

        if ($("#Cantidad").val() == "") {
            alert("Debe ingresar cantidad.");
            $("#Cantidad").focus();
            return false;
        }

        if ($("#Observaciones").val() == "") {
            alert("Debe ingresar una observación.");
            $("#Observaciones").focus(); 7
            return false;
        }

        $.post("/SAG_5/MovimientosBodega/GuardarMovimiento", $("form").serialize(),
            function (data) {
                if (data == "OK") {
                    $('body').css('opacity','0.25');
                    alert("Movimiento ingresado con con exito!");
                    $('body').css('opacity','1');
                    var currSrc = "/SAG_5/MovimientosBodega/ListarMovimientos?Periodo=" + periodo + "&Mes=" + mes;
                    $("#iframeBodega").attr("src", currSrc);
                    $("#iframeBodega").show();
                    /*$("form").each(function () {
                        this.reset();
                    });*/
                    $("#ArticuloID").val("");
                    $("#Cantidad").val("");
                    $("#Observaciones").val("");
                    $("#NroDocumento").val("");
                    $("#DocumentoID").val("");
                } else {
                    $('body').css('opacity','0.25');
                    alert("Ocurrió un error al guardar el movimiento.");
                    $('body').css('opacity','1');
                    return false;
                }
            })
            .success(function () { })
            .error(function () { })
            .complete(function () { });

        $("#PeriodoAnio").val(periodo);
        $("#PeriodoMes").val(mes);
    });

    $("#LimpiarFormulario").click(function () {
        $("form").each(function () {
            this.reset();
        });

        $("#MovimientosBodegaID").val("");
    });

    $("#BorrarMovimientoBodega").click(function () {
        var periodo = $("#PeriodoAnio").val();
        var mes = $("#PeriodoMes").val();

        if ($("#MovimientosBodegaID").val() != undefined && $("#MovimientosBodegaID").val() != "") {
            if (confirm("Realmente desea borrar este movimiento?")) {
                $.post("/SAG_5/MovimientosBodega/Delete", { mov: $("#MovimientosBodegaID").val() },
                function (data) {
                    if (data == "OK") {
                        $('body').css('opacity','0.25');
                        alert("Movimiento eliminado con con exito!");
                        $('body').css('opacity','1');
                        var currSrc = "/SAG_5/MovimientosBodega/ListarMovimientos";
                        $("#iframeBodega").attr("src", currSrc);
                        $("#iframeBodega").show();
                        
                        $("form").each(function () {
                            this.reset();
                        });

                        $("#MovimientosBodegaID").val("");
                    } else {
                        $('body').css('opacity','0.25');
                        alert("Ocurrió un error al eliminar el movimiento.");
                        $('body').css('opacity','1');
                        return false;
                    }
                })
                .success(function () { })
                .error(function () { })
                .complete(function () { });
            }
        } else {
            $('body').css('opacity','0.25');
            alert("Debe seleccionar un movimiento de bodega");
            $('body').css('opacity','1');
        }
        $("#PeriodoAnio").val(periodo);
        $("#PeriodoMes").val(mes);
    });



    // Detalle egresos
    $(".LineaDetalleEgreso").click(function () {
        window.parent.document.getElementById("NComprobanteDP").value = "";
        window.parent.document.getElementById("DocumentoIDD").value = "";
        window.parent.document.getElementById("NDocumentoD").value = "";
        window.parent.document.getElementById("Monto").value = "";
        window.parent.document.getElementById("FechaEmision").value = $(this).attr("FechaEmision");
        window.parent.document.getElementById("FechaVencimiento").value = $(this).attr("FechaVencimiento");
        window.parent.document.getElementById("CuentaID").value = "";
        window.parent.document.getElementById("Glosa").value = "";
        window.parent.document.getElementById("DeudaPendienteID").value = "";
        window.parent.document.getElementById("Origen").value = "";
        window.parent.document.getElementById("DeudaPendienteID").value = "";
        window.parent.document.getElementById("BoletaHonorarioID").value = "";

        if ($(this).attr("DetalleEgresoID") != undefined && $(this).attr("DetalleEgresoID") != "" && $(this).attr("DetalleEgresoID") != null && $(this).attr("DetalleEgresoID") != "0") {
            window.parent.document.getElementById("DetalleEgresoID").value = "";
            window.parent.document.getElementById("DetalleEgresoID").value = $(this).attr("DetalleEgresoID");
        } else {
            window.parent.document.getElementById("DetalleEgresoIndex").value = "";
            window.parent.document.getElementById("DetalleEgresoIndex").value = $(this).attr("DetalleEgresoIndex");
        }

        if ($(this).attr("DeudaPendienteID") != "") {
            // Detalle es deuda pendiente
            window.parent.document.getElementById("NComprobanteDP").value = $(this).attr("NComprobanteDP");
            window.parent.document.getElementById("DocumentoIDD").value = $(this).attr("DocumentoID");
            window.parent.document.getElementById("NDocumentoD").value = $(this).attr("NumeroDocumento");
            window.parent.document.getElementById("Monto").value = $(this).attr("Monto");
            window.parent.document.getElementById("FechaEmision").value = $(this).attr("FechaEmision");
            window.parent.document.getElementById("FechaVencimiento").value = $(this).attr("FechaVencimiento");
            window.parent.document.getElementById("CuentaID").value = $(this).attr("CuentaID");
            window.parent.document.getElementById("Glosa").value = $(this).attr("Glosa");
            window.parent.document.getElementById("DeudaPendienteID").value = $(this).attr("DeudaPendienteID");
            window.parent.document.getElementById("Origen").value = "dp";

            window.parent.document.getElementById("NComprobanteDP").setAttribute("readonly", "readonly");
            window.parent.document.getElementById("NDocumento").setAttribute("readonly", "readonly");
            window.parent.document.getElementById("FechaEmision").setAttribute("readonly", "readonly");
            window.parent.document.getElementById("FechaVencimiento").setAttribute("readonly", "readonly");
            window.parent.document.getElementById("DocumentoID").setAttribute("readonly", "readonly");
        } else if ($(this).attr("BoletaHonorarioID") != "") {
            window.parent.document.getElementById("FechaEmision").value = $(this).attr("FechaEmision");
            window.parent.document.getElementById("NDocumentoD").value = $(this).attr("NumeroDocumento");
            window.parent.document.getElementById("Monto").value = $(this).attr("Monto");
            window.parent.document.getElementById("Glosa").value = $(this).attr("Glosa");
            window.parent.document.getElementById("BoletaHonorarioID").value = $(this).attr("BoletaHonorarioID");
            window.parent.document.getElementById("Origen").value = "hs";

            window.parent.document.getElementById("NComprobanteDP").setAttribute("disabled", true);
            window.parent.document.getElementById("NDocumentoD").setAttribute("readonly", "readonly");
            window.parent.document.getElementById("FechaEmision").setAttribute("readonly", "readonly");
            window.parent.document.getElementById("FechaVencimiento").setAttribute("disabled", true);
            window.parent.document.getElementById("Monto").setAttribute("readonly", "readonly");
            window.parent.document.getElementById("DocumentoIDD").value = "3";
        } else if ($(this).attr("FondoFijoID") != "") {
            $('body').css('opacity','0.25');
            alert("El detalle de la caja chica no se puede modificar ya que se genera automáticamente.");
            $('body').css('opacity','1');
            return false;
        } else {
            // Otro tipo de detalle
            window.parent.document.getElementById("NComprobanteDP").setAttribute("disabled", true);

            window.parent.document.getElementById("DocumentoIDD").value = $(this).attr("DocumentoID");
            window.parent.document.getElementById("NDocumentoD").value = $(this).attr("NumeroDocumento");
            window.parent.document.getElementById("Monto").value = $(this).attr("Monto");
            window.parent.document.getElementById("FechaEmision").value = $(this).attr("FechaEmision");
            window.parent.document.getElementById("FechaVencimiento").value = $(this).attr("FechaVencimiento");
            window.parent.document.getElementById("CuentaID").value = $(this).attr("CuentaID");
            window.parent.document.getElementById("Glosa").value = $(this).attr("Glosa");
            window.parent.document.getElementById("DeudaPendienteID").value = $(this).attr("DeudaPendienteID");
            window.parent.document.getElementById("Origen").value = "oo";
        }

        window.parent.document.getElementById("MontoOriginal").value = $(this).attr("Monto");
        window.parent.document.getElementById("BorrarLinea").style.display = 'block';
    });

    $("#BorrarLinea").click(function(){
        if ($(".egreso #DetalleEgresoID") != undefined && $(".egreso #DetalleEgresoID").val() != "" && $(".egreso #DetalleEgresoID") != null && $(".egreso #DetalleEgresoID").val() != "0") {
            $(".egreso #DetalleEgresoID").val("");
        } else {
            $(".egreso #DetalleEgresoIndex").val("");
        }

        var currSrc = $("#iframeDetalle").attr("src");
        $("#iframeDetalle").attr("src", "");
        $("#iframeDetalle").attr("src", currSrc);
        $("#iframeDetalle").show();

        $(".egreso #NComprobanteDP").val("");
        $(".egreso #DocumentoID").val("");
        $(".egreso #NDocumento").val("");
        $(".egreso #Monto").val("");
        $(".egreso #CuentaID").val("");
        $(".egreso #Glosa").val("");
        $(".egreso #Origen").val("");
        $(".egreso #DeudaPendienteID").val("");
        $(".egreso #BoletaHonorarioID").val("");
        $("#BorrarLinea").hide();
    });


    // Conciliacion Bancaria
    $(".conciliar").click(function () {
        if ($("#vconciliar").val() == "c") {
            var detalleEgresoID = 0;
            var movimientoID = 0;
            var tr = $(this);
            var chequesNoCobrados = new Number($("#ChequesNoCobrados").val());
            var monto = new Number($(this).attr("Monto"));

            if ($(this).attr("detalleEgresoID") != undefined) {
                detalleEgresoID = $(this).attr("detalleEgresoID");
            }

            if ($(this).attr("movimientoID") != undefined) {
                movimientoID = $(this).attr("movimientoID");
            }

            $.post("/SAG_5/Banco/Conciliar", { detalleEgresoID: detalleEgresoID, movimientoID: movimientoID },
                function (data) {
                    if (data == "OK" || data == "OK2") {
                        tr.css('background-color', '');
                        if (data == "OK") {
                            tr.css('background-color', 'Orange');
                            chequesNoCobrados = chequesNoCobrados - monto;
                            $("#SaldoLibro").val((new Number($("#SaldoLibro").val())) + monto);
                        } else {
                            chequesNoCobrados = chequesNoCobrados + monto;
                            $("#SaldoLibro").val((new Number($("#SaldoLibro").val())) - monto);
                        }
                        $("#ChequesNoCobrados").val(chequesNoCobrados);
                    } else {
                        $('body').css('opacity', '0.25');
                        alert(data);
                        $('body').css('opacity', '1');
                        return false;
                    }
                })
                .success(function () { })
                .error(function () { })
                .complete(function () { });

        }});

    if ($("#totalNoConciliado") != undefined && $("#totalNoConciliado").val() != "") {
        var totalNoConcicliado = new Number($("#totalNoConciliado").val());
        $("#ChequesNoCobrados").val(totalNoConcicliado);
        $("#SaldoLibro").val((new Number($("#SaldoLibro").val())) - totalNoConcicliado);
    }

    $("#SaldoCartola").change(function () {
        var saldoCartola = 0;
        var saldoCartolaHidden = 0;
        if ($(this).val() != "") {
            saldoCartola = new Number($(this).val());
            saldoCartolaHidden = new Number($("#SaldoCartolaHidden").val());
            $("#SaldoLibro").val((new Number($("#SaldoLibro").val())) - saldoCartolaHidden + saldoCartola);
            $("#SaldoCartolaHidden").val(saldoCartola);
        }
    });

    $("#GastosBancarios").change(function () {
        var gastosBancarios = 0;
        var gastosBancariosHidden = 0;
        if ($(this).val() != "") {
            gastosBancarios = new Number($(this).val());
            gastosBancariosHidden = new Number($("#GastosBancariosHidden").val());
            $("#SaldoLibro").val((new Number($("#SaldoLibro").val())) - gastosBancariosHidden + gastosBancarios);
            $("#GastosBancariosHidden").val(gastosBancarios);
        }
    });

    $("#Depositos").change(function () {
        var depositos = 0;
        var depositosHidden = 0;
        if ($(this).val() != "") {
            depositos = new Number($(this).val());
            depositosHidden = new Number($("#DepositosHidden").val());
            $("#SaldoLibro").val((new Number($("#SaldoLibro").val())) - depositosHidden + depositos);
            $("#DepositosHidden").val(depositos);
        }
    });

    // Funciones para la creación de PROYECTOS

    $("#EdadMinima").change(function () {
        if ($("#EdadMinima").val() != "" && $("#EdadMaxima").val() != "") {
            var edadMinima = new Number($("#EdadMinima").val());
            var edadMaxima = new Number($("#EdadMaxima").val());

            if (edadMinima > edadMaxima) {
                alert("Debe ingresar edades válidas.");
                $("#EdadMinima").focus();
                return false;
            }

            if (edadMinima > 100) {
                alert("Debe ingresar edades válidas.");
                $("#EdadMinima").focus();
                return false;
            }
        }
    });

    $("#EdadMaxima").change(function () {
        if ($("#EdadMinima").val() != "" && $("#EdadMaxima").val() != "") {
            var edadMinima = new Number($("#EdadMinima").val());
            var edadMaxima = new Number($("#EdadMaxima").val());

            if (edadMinima > edadMaxima) {
                alert("Debe ingresar edades válidas.");
                $("#EdadMinima").focus();
                return false;
            }
        }
    });

    // Validación de campos obligatorios al ingresar nuevo proyecto por parte del administrador.
    $("#crearProyecto").click(function () {
        var CodCodeni = $("#CodCodeni").val();
        var Institucion = $("#Institucion").val();
        var Nombre = $("#Nombre").val();

        if (CodCodeni == "" || Institucion == "" || Nombre == "") {
            $('body').css('opacity','0.25');
            alert("Debe ingresar todos los campos marcados con azul.");
            $('body').css('opacity','1');
            return false;
        }

        var personaID = $("#PersonaID").val();
        if (personaID == "") {
            $('body').css('opacity','0.25');
            alert("Debe seleccionar un Supervisor para este Proyecto.");
            $('body').css('opacity','1');
            $("#PersonaID").focus();
            return false;
        }

        var BancoID = $("#BancoID").val();
        var Numero = $("#Numero").val();
        if (BancoID == "" || Numero == "") {
            if (confirm("Se recomienda ingresar una Cuenta Corriente, desea continuar sin ingresar esta información?")) {
                return true
            }
            $("#BancoID").focus();
            return false;
        }

        return true;
    });

    $(".validarPersona").click(function () {
        var Rut = $("#Rut").val();
        var DV = $("#DV").val();
        var Nombres = $("#Nombres").val();
        var ApellidoParterno = $("#ApellidoParterno").val();

        if (Rut == "" || DV == "" || Nombres == "" || ApellidoParterno == "") {
            $('body').css('opacity','0.25');
            alert("Debe ingresar todos los campos marcados con azul.");
            $('body').css('opacity','1');
            return false;
        }

        return true;
    });


    // Presupuesto

    $("#ActualizarPresupuesto").click(function(){
        window.location.reload();
        return false;
    });

    $("#periodoPresupuesto").change(function () {
        window.location = '/SAG_5/Presupuesto/Formulacion?Periodo=' + $(this).val();
        return false;
    });

    $("#periodoControl").change(function () {
        window.location = '/SAG_5/Presupuesto/Control?Periodo=' + $(this).val();
        return false;
    });

    $("#lineaProteccionControl").change(function () {
        var periodo = $("#periodoControlP").val();
        var l = $("#La").val();
        window.location = '/SAG_5/Presupuesto/ControlLinea?Periodo=' + periodo + '&LineaAtencion=' + $(this).val() + '&La=' + l;
        return false;
    });
    $("#periodoControlP").change(function () {
        var linea = $("#lineaProteccionControl").val();
        var l = $("#La").val();
        window.location = '/SAG_5/Presupuesto/ControlLinea?Periodo=' + $(this).val() + '&LineaAtencion=' + linea + '&La=' + l;
        return false;
    });
    $("#ProyectosIndicador").change(function () {
        var perio = $("#periodoIndicador").val();
        window.location = '/SAG_5/Control/IndicadoresSag?Periodo=' + perio + '&PrIndicadores=' + $(this).val();
        return false;
    });
    $("#periodoIndicador").change(function () {
        var perio = $("#ProyectosIndicador").val();
        window.location = '/SAG_5/Control/IndicadoresSag?Periodo=' + $(this).val() + '&PrIndicadores=' + perio;
        return false;
    });
    $("#periodoBalance").change(function () {
        window.location = '/SAG_5/Presupuesto/Balance?Periodo=' + $(this).val();
        return false;
    });

    $("#lineaPeriodo").change(function () {
        var periodo = $(this).val();
        var linea = $("#lineaProteccion").val();
        window.location = '/SAG_5/Presupuesto/LineaProteccion?Periodo=' + periodo + '&Linea=' + linea;
        return false;
    });

    $("#lineaPeriodoR").change(function () {
        var periodo = $(this).val();
        var linea = $("#lineaResponsabilidad").val();
        window.location = '/SAG_5/Presupuesto/LineaResponsabilidad?Periodo=' + periodo + '&Linea=' + linea;
        return false;
    });

    

    $("#lineaProteccion").change(function () {
        var periodo = $("#lineaPeriodo").val();
        var linea = $("#lineaProteccion").val();
        window.location = '/SAG_5/Presupuesto/LineaProteccion?Periodo=' + periodo + '&Linea=' + linea;
        return false;
    });
    $("#lineaEERR").change(function () {
        var periodo = $("#lineaPeriodo").val();
        var linea = $("#lineaEERR").val();
        window.location = '/SAG_5/Presupuesto/LineaPrevencion?Periodo=' + periodo + '&Linea=' + linea;
        return false;
    });
    $("#Excelsaldo").click(function () {
        var periodo = $("#Periodo").val();
        var mes = $("#Mes").val();
        window.open("/SAG_5/Imprimir/Excelsaldo?Periodo=" + periodo + '&Mes=' + mes, 'LibroBanco');
        return false;
    });
    $("#ExcelListaProyecto").click(function () {
        var inicio = $("#Desde").val();
        var fin = $("#Hasta").val();
        var tproyecto = $("#TProyecto").val();
        window.open("/SAG_5/Presupuesto/LineaProyectoExcel?inicio=" + inicio + '&fin=' + fin + '&Linea=' + tproyecto, 'LibroBanco');
        return false;
    });
    $("#ExcelListaProyectoAnual").click(function () {
        var desde = $("#anual").val();
        var hasta = $("#TProyecto").val();
        window.open("/SAG_5/Presupuesto/LineaProyectoExcelAnual?anual=" + anual + '&Linea=' + tproyecto, 'LibroBanco');
        return false;
    });

    $("#lineaResponsabilidad").change(function () {
        var periodo = $("#lineaPeriodoR").val();
        var linea = $("#lineaResponsabilidad").val();
        window.location = '/SAG_5/Presupuesto/LineaResponsabilidad?Periodo=' + periodo + '&Linea=' + linea;
        return false;
    }); 
    

    $("#exportarPresupuestoExcel").change(function () {
        var value = $(this).val();
        var periodo = $("#periodoControl").val();
        var pr_id = $("#Proyectos2").val();
        var td = $("#Texcel").val();
        if (value != "") {
            if (value == "Anual") {
              
                if (td == "n") {
                    window.open('/SAG_5/Presupuesto/Excel?Periodo=' + periodo + '&pr_id=' + pr_id, '', '');
                }
                if (td == "r") {               
                    window.open('/SAG_5/Presupuesto/ExcelResumenProyecto?Periodo=' + periodo + '&pr_id=' + pr_id, '', '');
                }

            } else if (value == "1S") {
                if (td == "n") {
                    window.open('/SAG_5/Presupuesto/ExcelSemestre1?Periodo=' + periodo + '&pr_id=' + pr_id, '', '');
                }
                if (td == "r") {
                    window.open('/SAG_5/Presupuesto/ExcelResumenSemestre1?Periodo=' + periodo + '&pr_id=' + pr_id, '', '');
                }
            } else if (value == "2S") {
                if (td == "n") {
                    window.open('/SAG_5/Presupuesto/ExcelSemestre2?Periodo=' + periodo + '&pr_id=' + pr_id, '', '');
                }
                if (td == "r") {
                    window.open('/SAG_5/Presupuesto/ExcelResumenSemestre2?Periodo=' + periodo + '&pr_id=' + pr_id, '', '');
                }


    
            } else if (value == "1T") {
                window.open('/SAG_5/Presupuesto/Excel', '', '');
            } else if (value == "2T") {
                window.open('/SAG_5/Presupuesto/Excel', '', '');
            } else if (value == "3T") {
                window.open('/SAG_5/Presupuesto/Excel', '', '');
            } else if (value == "4T") {
                window.open('/SAG_5/Presupuesto/Excel', '', '');
            }
        }

        $(this).val("");

        return false;
    });
    $("#exportarControlPresupuestoExcel").change(function () {
        var value = $(this).val();
        var periodo = $("#periodoControlP").val();
        var linea = $("#lineaProteccionControl").val();
      
       
        var td = $("#Texcel").val();
        if (value != "") {
            if (value == "Anual") {
                if (td == "n") {
                    window.open('/SAG_5/Presupuesto/ExcelLinea?Periodo=' + periodo + '&linea=' + linea, '', '');
                }
                if (td == "r") {
                    window.open('/SAG_5/Presupuesto/ExcelLineaResumen?Periodo=' + periodo + '&linea=' + linea, '', '');
                }
                if (td == "f") {
                    window.open('/SAG_5/Presupuesto/ExcelLineaResumenInforme?Periodo=' + periodo + '&linea=' + linea, '', '');
                }
                if (td == "s") {
                    window.open('/SAG_5/Presupuesto/ExcelLineaResumenInformeSinte?Periodo=' + periodo + '&linea=' + linea, '', '');
                }
            } else if (value == "1S") {
                if (td == "n") {
                    window.open('/SAG_5/Presupuesto/ExcelLineaSemestral?Periodo=' + periodo + '&linea=' + linea + '&se=1' , '', '');
                }
                if (td == "r") {
                    window.open('/SAG_5/Presupuesto/ExcelLineaResumenSemestral?Periodo=' + periodo + '&linea=' + linea + '&se=1', '', '');
                }
                if (td == "f") {
                    window.open('/SAG_5/Presupuesto/ExcelLineaResumenInformeSemestral?Periodo=' + periodo + '&linea=' + linea + '&se=1', '', '');
                }
                if (td == "s") {
                    window.open('/SAG_5/Presupuesto/ExcelLineaResumenInformeSinteSemestral?Periodo=' + periodo + '&linea=' + linea + '&se=1', '', '');
                }
            } else if (value == "2S") {
                if (td == "n") {
                    window.open('/SAG_5/Presupuesto/ExcelLineaSemestral?Periodo=' + periodo + '&linea=' + linea + '&se=7', '', '');
                }
                if (td == "r") {
                    window.open('/SAG_5/Presupuesto/ExcelLineaResumenSemestral?Periodo=' + periodo + '&linea=' + linea + '&se=7', '', '');
                }
                if (td == "f") {
                    window.open('/SAG_5/Presupuesto/ExcelLineaResumenInformeSemestral?Periodo=' + periodo + '&linea=' + linea + '&se=7', '', '');
                }
                if (td == "s") {
                    window.open('/SAG_5/Presupuesto/ExcelLineaResumenInformeSinteSemestral?Periodo=' + periodo + '&linea=' + linea + '&se=7', '', '');
                }          
            } else if (value == "1T") {
                if (td == "n") {
                    window.open('/SAG_5/Presupuesto/ExcelLineaTrimestral?Periodo=' + periodo + '&linea=' + linea + '&tri=1', '', '');
                }
                if (td == "r") {
                    window.open('/SAG_5/Presupuesto/ExcelLineaResumenTrimestral?Periodo=' + periodo + '&linea=' + linea + '&tri=1', '', '');
                }
                if (td == "f") {
                    window.open('/SAG_5/Presupuesto/ExcelLineaResumenInformeTrimestral?Periodo=' + periodo + '&linea=' + linea + '&tri=1', '', '');
                }
                if (td == "s") {
                    window.open('/SAG_5/Presupuesto/ExcelLineaResumenInformeSinteTrimestral?Periodo=' + periodo + '&linea=' + linea + '&tri=1' , '', '');
                }
            } else if (value == "2T") {
                if (td == "n") {
                    window.open('/SAG_5/Presupuesto/ExcelLineaTrimestral?Periodo=' + periodo + '&linea=' + linea + '&tri=4', '', '');
                }
                if (td == "r") {
                    window.open('/SAG_5/Presupuesto/ExcelLineaResumenTrimestral?Periodo=' + periodo + '&linea=' + linea + '&tri=4', '', '');
                }
                if (td == "f") {
                    window.open('/SAG_5/Presupuesto/ExcelLineaResumenInformeTrimestral?Periodo=' + periodo + '&linea=' + linea + '&tri=4', '', '');
                }
                if (td == "s") {
                    window.open('/SAG_5/Presupuesto/ExcelLineaResumenInformeSinteTrimestral?Periodo=' + periodo + '&linea=' + linea + '&tri=4', '', '');
                }
            } else if (value == "3T") {
                if (td == "n") {
                    window.open('/SAG_5/Presupuesto/ExcelLineaTrimestral?Periodo=' + periodo + '&linea=' + linea + '&tri=7', '', '');
                }
                if (td == "r") {
                    window.open('/SAG_5/Presupuesto/ExcelLineaResumenTrimestral?Periodo=' + periodo + '&linea=' + linea + '&tri=7', '', '');
                }
                if (td == "f") {
                    window.open('/SAG_5/Presupuesto/ExcelLineaResumenInformeTrimestral?Periodo=' + periodo + '&linea=' + linea + '&tri=7', '', '');
                }
                if (td == "s") {
                    window.open('/SAG_5/Presupuesto/ExcelLineaResumenInformeSinteTrimestral?Periodo=' + periodo + '&linea=' + linea + '&tri=7', '', '');
                }
            } else if (value == "4T") {
                if (td == "n") {
                    window.open('/SAG_5/Presupuesto/ExcelLineaTrimestral?Periodo=' + periodo + '&linea=' + linea + '&tri=10', '', '');
                }
                if (td == "r") {
                    window.open('/SAG_5/Presupuesto/ExcelLineaResumenTrimestral?Periodo=' + periodo + '&linea=' + linea + '&tri=10', '', '');
                }
                if (td == "f") {
                    window.open('/SAG_5/Presupuesto/ExcelLineaResumenInformeTrimestral?Periodo=' + periodo + '&linea=' + linea + '&tri=10', '', '');
                }
                if (td == "s") {
                    window.open('/SAG_5/Presupuesto/ExcelLineaResumenInformeSinteTrimestral?Periodo=' + periodo + '&linea=' + linea + '&tri=10', '', '');
                }
            }
        }


        $(this).val("");

        return false;
    });
    $("#exportarControlPresupuestoExcelPR").change(function () {
        var value = $(this).val();
        var periodo = $("#periodoControlPR").val();
        var linea = $("#lineaProteccionControl").val();
       

        var td = $("#Texcel").val();
        if (value != "") {
            if (value == "Anual") {
                if (td == "n") {
                    window.open('/SAG_5/Presupuesto/ExcelLinea?Periodo=' + periodo + '&linea=' + linea, '', '');
                }
                if (td == "r") {
                    window.open('/SAG_5/Presupuesto/ExcelLineaResumen?Periodo=' + periodo + '&linea=' + linea, '', '');
                }
                if (td == "f") {
                    window.open('/SAG_5/Presupuesto/ExcelLineaResumenInforme?Periodo=' + periodo + '&linea=' + linea, '', '');
                }
            } else if (value == "1S") {
                if (td == "n") {
                    window.open('/SAG_5/Presupuesto/ExcelLinea?Periodo=' + periodo + '&linea=' + linea, '', '');
                }
                if (td == "r") {
                    window.open('/SAG_5/Presupuesto/ExcelLineaResumen?Periodo=' + periodo + '&linea=' + linea, '', '');
                }
                if (td == "f") {
                    window.open('/SAG_5/Presupuesto/ExcelLineaResumenInforme?Periodo=' + periodo + '&linea=' + linea, '', '');
                }
            } else if (value == "2S") {
                if (td == "n") {
                    window.open('/SAG_5/Presupuesto/ExcelLinea?Periodo=' + periodo + '&linea=' + linea, '', '');
                }
                if (td == "r") {
                    window.open('/SAG_5/Presupuesto/ExcelLineaResumen?Periodo=' + periodo + '&linea=' + linea, '', '');
                }
                if (td == "f") {
                    window.open('/SAG_5/Presupuesto/ExcelLineaResumenInforme?Periodo=' + periodo + '&linea=' + linea, '', '');
                }
            } else if (value == "2S") {
                if (td == "n") {
                    window.open('/SAG_5/Presupuesto/ExcelLinea?Periodo=' + periodo + '&linea=' + linea, '', '');
                }
                if (td == "r") {
                    window.open('/SAG_5/Presupuesto/ExcelLineaResumen?Periodo=' + periodo + '&linea=' + linea, '', '');
                }
                if (td == "f") {
                    window.open('/SAG_5/Presupuesto/ExcelLineaResumenInforme?Periodo=' + periodo + '&linea=' + linea, '', '');
                }
            } else if (value == "1T") {
                if (td == "n") {
                    window.open('/SAG_5/Presupuesto/ExcelLinea?Periodo=' + periodo + '&linea=' + linea, '', '');
                }
                if (td == "r") {
                    window.open('/SAG_5/Presupuesto/ExcelLineaResumen?Periodo=' + periodo + '&linea=' + linea, '', '');
                }
                if (td == "f") {
                    window.open('/SAG_5/Presupuesto/ExcelLineaResumenInforme?Periodo=' + periodo + '&linea=' + linea, '', '');
                }
            } else if (value == "2T") {
                if (td == "n") {
                    window.open('/SAG_5/Presupuesto/ExcelLinea?Periodo=' + periodo + '&linea=' + linea, '', '');
                }
                if (td == "r") {
                    window.open('/SAG_5/Presupuesto/ExcelLineaResumen?Periodo=' + periodo + '&linea=' + linea, '', '');
                }
                if (td == "f") {
                    window.open('/SAG_5/Presupuesto/ExcelLineaResumenInforme?Periodo=' + periodo + '&linea=' + linea, '', '');
                }
            } else if (value == "3T") {
                if (td == "n") {
                    window.open('/SAG_5/Presupuesto/ExcelLinea?Periodo=' + periodo + '&linea=' + linea, '', '');
                }
                if (td == "r") {
                    window.open('/SAG_5/Presupuesto/ExcelLineaResumen?Periodo=' + periodo + '&linea=' + linea, '', '');
                }
                if (td == "f") {
                    window.open('/SAG_5/Presupuesto/ExcelLineaResumenInforme?Periodo=' + periodo + '&linea=' + linea, '', '');
                }
            } else if (value == "4T") {
                if (td == "n") {
                    window.open('/SAG_5/Presupuesto/ExcelLinea?Periodo=' + periodo + '&linea=' + linea, '', '');
                }
                if (td == "r") {
                    window.open('/SAG_5/Presupuesto/ExcelLineaResumen?Periodo=' + periodo + '&linea=' + linea, '', '');
                }
                if (td == "f") {
                    window.open('/SAG_5/Presupuesto/ExcelLineaResumenInforme?Periodo=' + periodo + '&linea=' + linea, '', '');
                }
            }
        }


        $(this).val("");

        return false;
    });
    $("#exportarBalanceExcel").change(function () {
        var value = $(this).val();
        var periodo = $("#periodoBalance").val();
        var pr_id = $("#Proyectos2").val();
       
        if (value != "") {
            if (value == "Anual") {
                window.open('/SAG_5/Presupuesto/ExcelBalance?Periodo=' + periodo + '&pr_id=' + pr_id, '', '');
            } else if (value == "1S") {
                window.open('/SAG_5/Presupuesto/ExcelBalanceSemestre1?Periodo=' + periodo + '&pr_id=' + pr_id, '', '');
            } else if (value == "2S") {
                window.open('/SAG_5/Presupuesto/ExcelBalanceSemestre2?Periodo=' + periodo + '&pr_id=' + pr_id, '', '');
            } else if (value == "2S") {
                window.open('/SAG_5/Presupuesto/Excel', '', '');
            } else if (value == "1T") {
                window.open('/SAG_5/Presupuesto/Excel', '', '');
            } else if (value == "2T") {
                window.open('/SAG_5/Presupuesto/Excel', '', '');
            } else if (value == "3T") {
                window.open('/SAG_5/Presupuesto/Excel', '', '');
            } else if (value == "4T") {
                window.open('/SAG_5/Presupuesto/Excel', '', '');
            }
        }

        $(this).val("");

        return false;
    });

    $("#excelBalanceLinea").click(function () {
        var lineaPeriodo = $("#lineaPeriodo").val();
        var lineaProteccion = $("#lineaProteccion").val();

        window.open('/SAG_5/Presupuesto/LineaProteccionExcel?Periodo=' + lineaPeriodo + '&Linea=' + lineaProteccion, '', '');
        return false;
    });
    $("#excelBalanceLineaEERR").click(function () {
        var lineaPeriodo = $("#lineaPeriodo").val();
        var lineaProteccion = $("#lineaEERR").val();

        window.open('/SAG_5/Presupuesto/LineaProteccionExcel?Periodo=' + lineaPeriodo + '&Linea=' + lineaProteccion, '', '');
        return false;
    });
    $("#excelBalanceResponsabilidad").click(function () {
        var lineaPeriodoR = $("#lineaPeriodoR").val();
        var lineaResponsabilidad = $("#lineaResponsabilidad").val();

        window.open('/SAG_5/Presupuesto/LineaResponsabilidadExcel?Periodo=' + lineaPeriodoR + '&Linea=' + lineaResponsabilidad, '', '');
        return false;
    });

    $(".Presupuesto").click(function () {
        var campo = $(this);
        if (campo.val() == "0") {
            campo.val("");
        }
    });

    $(".Presupuesto").blur(function () {
        var campo = $(this);
        if (campo.val() == "") {
            campo.val("0");
        }
    });

    $(".Presupuesto").change(function () {
        var padre = $(this);
        var tienePadre = true;
        while (tienePadre) {
            if (padre.attr("padre") != undefined && padre.attr("padre") != "1" && padre.attr("padre") != "6") {
                padreID = padre.attr("padre");
                mes = padre.attr("mes");
                campoPadre = "Presupuesto_" + mes + "_" + padreID;
                valorAnterior = new Number(padre.attr("valorAnterior"));
                valorPadre = new Number($("#" + campoPadre).val());
                valorHijo = new Number(padre.val());
                $("#" + campoPadre).val(valorPadre + valorHijo - valorAnterior);
                padre.attr("valorAnterior", padre.val());
                padre = $("#" + campoPadre);
            } else {
                tienePadre = false;
            }
        }
    });

    // Cargamos los valores del presupuesto
    $(".Presupuesto").each(function () {
        var actual = $(this);
        var tienePadre = true;
        if (actual.val() != "0" && actual.attr("eshoja") != undefined) {
            var valorHoja = new Number(actual.val());
            while (tienePadre) {
                //alert(padre.val());
                if (actual.attr("padre") != undefined && actual.attr("padre") != "1" && actual.attr("padre") != "6") {
                    padreID = actual.attr("padre");
                    mes = actual.attr("mes");
                    campoPadre = "Presupuesto_" + mes + "_" + padreID;

                    valorActual = new Number(actual.val());
                    valorAnterior = new Number(actual.attr("valorAnterior"));
                    valorPadre = new Number($("#" + campoPadre).val());
                    
                    $("#" + campoPadre).val(valorPadre + valorHoja);
                    actual.attr("valorAnterior", actual.val());
                    actual = $("#" + campoPadre);
                } else {
                    tienePadre = false;
                }
            }
        }
    });

    // Cargamos los valores presupuestados en el Control de Presupuesto
    /*
    $(".PresupuestoControl").each(function () {
        var actual = $(this);
        var tienePadre = true;
        if (actual.val() != "0" && actual.attr("eshoja") != undefined) {
            var valorHoja = new Number(actual.html().split(".").join(""));
            while (tienePadre) {
                if (actual.attr("padre") != undefined && actual.attr("padre") != "1" && actual.attr("padre") != "6") {
                    padreID = actual.attr("padre");
                    mes = actual.attr("mes");
                    campoPadre = "Presupuesto_" + mes + "_" + padreID;
                    valorPadre = new Number($("#" + campoPadre).html().split(".").join(""));
                    $("#" + campoPadre).html(addPeriod(valorPadre + valorHoja));
                    actual = $("#" + campoPadre);
                } else {
                    tienePadre = false;
                }
            }
        }
    });

    $(".PresupuestoReal").each(function () {
        var actual = $(this);
        var tienePadre = true;
        if (actual.val() != "0" && actual.attr("eshoja") != undefined) {
            var valorHoja = new Number(actual.html().split(".").join(""));
            while (tienePadre) {
                if (actual.attr("padre") != undefined && actual.attr("padre") != "1" && actual.attr("padre") != "6") {
                    padreID = actual.attr("padre");
                    mes = actual.attr("mes");
                    campoPadre = "PresupuestoReal_" + mes + "_" + padreID;
                    valorPadre = new Number($("#" + campoPadre).html().split(".").join(""));
                    $("#" + campoPadre).html(addPeriod(valorPadre + valorHoja));
                    actual = $("#" + campoPadre);
                } else {
                    tienePadre = false;
                }
            }
        }
    });

    $(".PresupuestoDesviacion").each(function () {
        var actual = $(this);
        var tienePadre = true;
        if (actual.val() != "0" && actual.attr("eshoja") != undefined) {
            var valorHoja = new Number(actual.html().split(".").join(""));
            while (tienePadre) {
                if (actual.attr("padre") != undefined && actual.attr("padre") != "1" && actual.attr("padre") != "6") {
                    padreID = actual.attr("padre");
                    mes = actual.attr("mes");
                    campoPadre = "PresupuestoDesviacion_" + mes + "_" + padreID;
                    valorPadre = new Number($("#" + campoPadre).html().split(".").join(""));
                    $("#" + campoPadre).html(addPeriod(valorPadre + valorHoja));
                    actual = $("#" + campoPadre);
                } else {
                    tienePadre = false;
                }
            }
        }
    });

     $(".PresupuestoPorcentaje").each(function () {
        var actual = $(this);
        var mes = actual.attr("mes");
        var cuentaID = actual.attr("cuentaID");
        var tipo = actual.attr("tipo");
        var MontoReal = parseFloat($("#PresupuestoReal_" + mes + "_" + cuentaID).html().split(".").join(""));
        var MontoPresupuestado = parseFloat($("#Presupuesto_" + mes + "_" + cuentaID).html().split(".").join(""));
        
        if (MontoPresupuestado != 0) {
            if (tipo == "I") {
                $(this).html(((MontoReal - MontoPresupuestado) * 100 / MontoPresupuestado).toFixed(2) + "%");
            } else if (tipo == "E") {
                $(this).html(((MontoPresupuestado - MontoReal) * 100 / MontoPresupuestado).toFixed(2) + "%");
            }
        } else {
            $(this).html("-");
        }
    });*/

    $("#MesInicioPresupuesto").change(function () {
        var primerMes = new Number($(this).val());
        var j = 1;
        for (var i = primerMes - 1; i < 12; i++) {
            $(".Mes_" + j).html(mesesCorto[i]);
            j++;
        }
        for (var i = 0; i < primerMes - 1; i++) {
            $(".Mes_" + j).html(mesesCorto[i]);
            j++;
        }
    });

    // Replicar primer valor
    $(".ReplicarValor").click(function () {
        var cuentaID = $(this).attr("cuenta");
        var valor = $("#Presupuesto_1_" + cuentaID).val();

        if (valor != "") {
            for (var i = 2; i < 13; i++) {
                $("#Presupuesto_" + i + "_" + cuentaID).val(valor).change();
            }
        }
    });

    $("#TipoCuenta").change(function () {
        $(".cuenta_I").hide();
        $(".cuenta_E").hide();
        $(".cuenta_" + $("#TipoCuenta").val()).show();
    });


    $(".filtroPresupuesto").click(function () {
        var mesSeleccionado = $(this);
        var mesInicio = $("#mesInicio").val();

        if (mesInicio == mesSeleccionado.attr("mes")) {
            return false;
        }

        if (mesSeleccionado.is(':checked')) {
            // Mostrar todos los meses siguientes
            for (var i = mesSeleccionado.attr("mes") + 1 ; i < 12; i++) {
                $(".filtro_" + i).removeAttr("checked");
                $(".Periodo" + i).hide();
            }

            for (var i = mesSeleccionado.attr("mes"); i > 0; i--) {
                $(".filtro_" + i).attr("checked", "checked");
                $(".Periodo" + i).show();
            }
            //$(".Periodo" + mesSeleccionado.attr("mes")).show();
        } else {
            // Ocultar todos los meses siguientes
            for (var i = mesSeleccionado.attr("mes") ; i <= 12; i++) {
                $(".filtro_" + i).removeAttr("checked");
                $(".Periodo" + i).hide();
            }

            for (var i = mesSeleccionado.attr("mes") - 1; i > 0; i--) {
                $(".filtro_" + i).attr("checked", "checked");
                $(".Periodo" + i).show();
            }
            //$(".Periodo"+mesSeleccionado.attr("mes")).hide();
        }
    });

    // Click para expandir y ocultar cuentas
    $(".Mostrar").click(function(){
        /*var cuentaID = $(this).attr("cuentaID");
        var visible = $(this).attr("visible");
        if (visible == "true"){
            $(".Padre_" + cuentaID).hide();
            while ($(".Padre_" + cuentaID).attr("cuentaID") != undefined){
                cuentaID = $(".Padre_" + cuentaID).attr("cuentaID");
                $(".Padre_" + cuentaID).hide();
            }
            $(this).attr("visible", "false");
        } else {
            $(".Padre_" + cuentaID).show();
            while ($(".Padre_" + cuentaID).attr("cuentaID") != undefined){
                cuentaID = $(".Padre_" + cuentaID).attr("cuentaID");
                $(".Padre_" + cuentaID).show();
            }
            $(this).attr("visible", "true");
        }*/
    });

    $("#TipoCuenta").change();
    $("#MesInicioPresupuesto").change();
    $(".Presupuesto").change();

    // Solicitud de Anulacion para usuarios normales
    $(".SolicitaAnulacion").click(function () {
        var razon = prompt('Ingrese razon de anulacion del movimiento:');
        if (razon == "") {
            return false;
        } else {

        }
    });

    // Guardar datos del formulario de Egresos
    $("#linkGuardar").click(function () {
        if ($("#Cheque").val() == "") {
            $('body').css('opacity','0.25');
            alert("Debe ingresar numero de cheque.");
            $('body').css('opacity','1');
            $("#Cheque").focus();
        } else if ($("#Descripcion").val() == "") {
            $('body').css('opacity','0.25');
            alert("Debe ingresar glosa global.");
            $('body').css('opacity','1');
            $("#Descripcion").focus();
        } else if ($("#Monto_Egresos").val() == "") {
            $('body').css('opacity','0.25');
            alert("El egreso debe tener al menos una linea en el detalle.");
            $('body').css('opacity','1');
            $("#linkDetalle").click();
        } else {
            $("#botonGuardar").click();
        }
        return false;
    });


    $("#Guardar").click(function () {
        $("#botonGuardar").click();
        return false;
    });    


    $("#botonGuardar").click(function(){
        if($("#flagReitegro") != undefined && $("#flagReitegro").val() == "true") {
            if ($("#ComprobanteEgreso") == undefined || $("#ComprobanteEgreso").val() == "") {
                alert("Debe seleccionar un Comprobante de Egreso.");
                $("#ComprobanteEgreso").focus();
                return false;
            }
        }

        // Revisar fecha del comprobante
        var fecha = $("#Fecha").val();
        var periodo = $("#Periodo").val();
        var mes = $("#Mes").val();
        var datosFecha = fecha.split("-");

        if ($("#CajaChica").val() == undefined && (parseInt(datosFecha[1]) != parseInt(mes) || parseInt(datosFecha[2]) != parseInt(periodo))) {
            /*if(!confirm("La fecha seleccionada no corresponde al período activo, está seguro de que la fecha del comprobante es correcta?")){
                return false;
            }*/

            $('body').css('opacity', '0.25');
            alert("La fecha seleccionada no corresponde al período activo, no es posible guardar el comprobante!");
            $('body').css('opacity', '1');
            return false;
        }

        if ($("#PreguntarImprimir") != undefined && $("#PreguntarImprimir").val() == "true")
        {
            $('body').css('opacity','0.25');
            if (confirm("Desea imprimir el comprobante?"))
            {
                $("#ImprimirComprobante").val("true");
            }
            $('body').css('opacity','1');
        }
        return true;
    });

    //

    /*
    * Funciones para INFORMES
    */

    $(".imprimirInformePeriodo").click(function () {
        if ($("#Desde").val() == "" || $("#Hasta").val() == "") {
            $('body').css('opacity','0.25');
            alert("Debe seleccionar período");
            $('body').css('opacity','1');
            $("#Desde").focus();
        } else {
            var href = $(".imprimirInformePeriodo").attr("href");
            if ($("#TipoClasificacion").val() != undefined) {
                href = href + "&Clasificacion=" + $("#TipoClasificacion").val();
            }
            window.open(href, 'Buscar', 'target="_blank",width=750,height=400,scrollbars=yes,menubar=no,toolbars=no');
        }

        return false;
    });

    // Exportar Informes a Excel
    $(".excelBotonPeriodo").click(function () {
        var tipo = $(this).attr("tipo");

        if ($("#Desde").val() == undefined && $("#Hasta").val() == undefined) {
            window.open('/SAG_5/Informes/Excel' + tipo + '?Mes=' + $("#Mes").val() + '&Periodo=' + $("#Periodo").val(), '', '');
        } else {
            window.open('/SAG_5/Informes/Excel' + tipo + '?Desde=' + $("#Desde").val() + '&Hasta=' + $("#Hasta").val(), '', '');
        }

        return false;

    });

    $(".excelIndicador1").click(function () {
        var periodo = $("#periodoIndicador").val();
        var proyectoid = $("#ProyectosIndicador").val();

        window.open('/SAG_5/Control/IndicadoresExcel?Periodo=' + periodo + '&PrIndicadores=' + proyectoid);
     
        return false;


    });
    $("#Concepto").focusout(function () {
        var text = $("#Concepto").val();
        text = text.replace(/\r?\n/g, ' ');
        $("#Concepto").val(text);
    });

    $(".imprimirBotonPeriodo").click(function(){
        $(".imprimirInformePeriodo").click();
    });

    $(".imprimirInformeMes").click(function () {
        if ($("#Mes").val() == "" || $("#Periodo").val() == "") {
            $('body').css('opacity','0.25');
            alert("Debe seleccionar período");
            $('body').css('opacity','1');
            $("#Mes").focus();
        } else {
            var href = $(".imprimirInformeMes").attr("href");
            window.open(href, 'Buscar', 'width=750,height=400,scrollbars=yes,menubar=no,toolbars=no');
        }

        return false;
    });

    $("#ImprimirReporteHH").click(function(){
        if ($("#Mes").val() == "" || $("#Periodo").val() == "") {
            $('body').css('opacity','0.25');
            alert("Debe seleccionar período");
            $('body').css('opacity','1');
            $("#Mes").focus();
        } else {
            window.open('/SAG_5/BoletasHonorarios/ReporteImprimir?Mes='+$("#Mes").val()+'&Periodo='+$("#Periodo").val(), 'Reporte', 'width=750,height=400,scrollbars=yes,menubar=no,toolbars=no');
        }
        return false;
    });

    $(".imprimirBotonMes").click(function(){
        $(".imprimirInformeMes").click();
    });

    //
    // Rendicion de Cuentas > Deuda Pendiente
    //

    $(".BuscarPopUpPersonal").click(function(){
        if ($(".tipobeneficiario #tipoBeneficiario").val() == "1") {
            window.open("/SAG_5/Personal/PopUp", 'BuscarPersona', 'width=750,height=350,scrollbars=yes,menubar=no,toolbars=no');
        } else if ($(".tipobeneficiario #tipoBeneficiario").val() == "2") {
            window.open("/SAG_5/Proveedores/PopUp", 'BuscarProveedor', 'width=750,height=350,scrollbars=yes,menubar=no,toolbars=no');
        }
        return false;
    });

    $(".tipobeneficiario #DVBuscar").focusout(function(){
        $("#PersonaID").val("0");
        $("#ProveedorID").val("0");
        $("#Beneficiario").val("");

        if ($("#DVBuscar").val() != "" && $("#Rut").val() != "" && $(".tipobeneficiario #tipoBeneficiario").val() != "3") {
            if ($(".tipobeneficiario #tipoBeneficiario").val() == "1") {
                $.get('/SAG_5/DeudasPendientes/Personal?rut=' + $("#Rut").val()+'&dv='+$("#DVBuscar").val(), function (response) {
                    var personal = $.parseJSON(response);
                    if (personal.length == 0) {
                        if (confirm("La persona ingresada no existe. Desea ingresarla ahora?")) {
                            window.open("/SAG_5/Personal/IngresarPopUp?rut=" + $("#Rut").val()+'&dv='+$("#DVBuscar").val(), 'IngresarPersona', 'width=765,height=360');
                        } else {
                            return;
                        }
                    } else {
                        $("#PersonaID").val($.trim(personal[0].Id));
                        $("#ProveedorID").val("0");
                        var NombreCompleto = $.trim(personal[0].Nombre)+" "+$.trim(personal[0].Paterno)+" "+$.trim(personal[0].Materno);
                        $("#Beneficiario").val(NombreCompleto);
                    }
                });
            } else {
                $.get('/SAG_5/DeudasPendientes/Proveedores?rut=' + $("#Rut").val()+'&dv='+$("#DVBuscar").val(), function (response) {
                    var proveedores = $.parseJSON(response);
                    if (proveedores.length == 0) {
                        if (confirm("El proveedor ingresado no existe. Desea ingresarlo ahora?")) {
                            window.open("/SAG_5/Proveedores/IngresarPopUp?rut=" + $("#Rut").val()+'&dv='+$("#DVBuscar").val(), 'IngresarProveedor', 'width=765,height=400');
                        } else {
                            return;
                        }
                    } else {
                        $("#ProveedorID").val($.trim(proveedores[0].Id));
                        $("#PersonaID").val("0");
                        $("#Beneficiario").val($.trim(proveedores[0].Nombre));
                    }
                });
            }
        }
    });

    $("#ImprimirLibroBanco").click(function(){
        var periodo = $("#Periodo").val();
        var mes = $("#Mes").val();
        window.open("/SAG_5/Imprimir/LibroBanco?Periodo=" + periodo+'&Mes='+mes, 'LibroBanco');
        return false;
    });

    $("#ExcelLibroBanco").click(function () {
        var periodo = $("#Periodo").val();
        var mes = $("#Mes").val();
        window.open("/SAG_5/Imprimir/ExcelLibroBanco?Periodo=" + periodo + '&Mes=' + mes, 'LibroBanco');
        return false;
    });

    $("#ExportarConciliacion").click(function () {
        var periodo = $("#Periodo").val();
        var mes = $("#Mes").val();
        window.open("/SAG_5/Banco/Exportar?periodo=" + periodo + '&mes=' + mes, 'LibroBanco');
        return false;
    }); 

    /*
    $("#ImprimirConciliacion").click(function () {
        alert("asads");
        dataString = $("form").serialize();

        $.ajax({
            type: "POST",
            url: "/SAG_5/Banco/Conciliacion",
            data: dataString,
            dataType: "json",
            cache: false,
            error: function (jqXHR, exception) {
                alert(jqXHR.status);
            },
            success: function (data) {
                //alert(data.toString());
                var periodo = $("#Periodo").val();
                var mes = $("#Mes").val();
                window.open("/SAG_5/Imprimir/Conciliacion?Periodo=" + periodo + '&Mes=' + mes, 'Conciliacion');
                alert("asads");
            }
        });
        return false;
    });
    */
    $("#SaldoCartola").change();
    $("#GastosBancarios").change();
    $("#Depositos").change();

    // Auditorias
    $(".MenuItem").click(function () {
        // Agregar on click en radiobutton!!!!
        var itemSeleccionado = $(this).attr("div");
        var programaID = $("ProgramaID").val();

        dataString = $("form").serialize();

        $.ajax({
            type: "POST",
            url: "/SAG_5/Auditorias/Informe?Documentos=NO",
            data: dataString,
            cache: false,
            dataType: "json",
            success: function (data) { }
        });

        $(".Informe_Objetivos").hide();
        $(".Informe_Alcance").hide();
        $(".Informe_Metodologia").hide();
        $(".Informe_Seguimiento").hide();
        $(".Informe_Resultados").hide();
        $(".Informe_Conclusiones").hide();
        $(".Informe_Plazo").hide();
        $(".Informe_Antecedentes").hide();
        $("."+itemSeleccionado).show();
    });

    $("#informeGeneral").click(function(){
        window.location = "/SAG_5/Auditorias/ReporteGeneral/?DesdeMes=" + $("#DesdeMes").val() + "&DesdePeriodo=" + $("#DesdePeriodo").val() + "&HastaMes=" + $("#HastaMes").val() + "&HastaPeriodo=" + $("#HastaPeriodo").val();
    });

    $("#informeGrupal").click(function(){
        window.location = "/SAG_5/Auditorias/ReporteGrupal/?DesdeMes=" + $("#DesdeMes").val() + "&DesdePeriodo=" + $("#DesdePeriodo").val() + "&HastaMes=" + $("#HastaMes").val() + "&HastaPeriodo=" + $("#HastaPeriodo").val() + "&tipoProyectoID=" + $("#TipoProyectoID").val();
    });


    // Calculo punto 7. Provisión de Fondos para Indemnización
    $("#VI_7_b_Costos").change(function () {
        var superavit = 0;

        if (!isNaN($("#VI_7_b_Costos").val()) && $("#VI_7_b_Costos").val() != '') {
            superavit -= parseInt($("#VI_7_b_Costos").val());
        }

        if (!isNaN($("#VI_7_c_Fondos").val()) && $("#VI_7_c_Fondos").val() != '') {
            superavit += parseInt($("#VI_7_c_Fondos").val());
        }

        if (!isNaN($("#VI_7_d_Monto").val()) && $("#VI_7_d_Monto").val() != '') {
            superavit += parseInt($("#VI_7_d_Monto").val());
        }

        $("#VI_7_e_Superavit").val(superavit);
    });

    $("#VI_7_c_Fondos").change(function () {
        var superavit = 0;

        if (!isNaN($("#VI_7_b_Costos").val()) && $("#VI_7_b_Costos").val() != '') {
            superavit -= parseInt($("#VI_7_b_Costos").val());
        }

        if (!isNaN($("#VI_7_c_Fondos").val()) && $("#VI_7_c_Fondos").val() != '') {
            superavit += parseInt($("#VI_7_c_Fondos").val());
        }

        if (!isNaN($("#VI_7_d_Monto").val()) && $("#VI_7_d_Monto").val() != '') {
            superavit += parseInt($("#VI_7_d_Monto").val());
        }

        $("#VI_7_e_Superavit").val(superavit);
    });

    $("#VI_7_d_Monto").change(function () {
        var superavit = 0;

        if (!isNaN($("#VI_7_b_Costos").val()) && $("#VI_7_b_Costos").val() != '') {
            superavit -= parseInt($("#VI_7_b_Costos").val());
        }

        if (!isNaN($("#VI_7_c_Fondos").val()) && $("#VI_7_c_Fondos").val() != '') {
            superavit += parseInt($("#VI_7_c_Fondos").val());
        }

        if (!isNaN($("#VI_7_d_Monto").val()) && $("#VI_7_d_Monto").val() != '') {
            superavit += parseInt($("#VI_7_d_Monto").val());
        }

        $("#VI_7_e_Superavit").val(superavit);
    });

    // 4. Intervenciones realizadas en el periodo
    $(".cobertura").change(function () {
        var total = 0;
        var i = 1;

        $(".cobertura").each(function (index) {
            if (!isNaN($(this).val()) && $(this).val() != '') {
                total += parseInt($(this).val());
            }

            if (!isNaN($("#VI_C_4a_Cob" + i).val()) && $("#VI_C_4a_Cob" + i).val() != '' && !isNaN($("#VI_C_4b_Int" + i).val()) && $("#VI_C_4b_Int" + i).val() != '') {
                var int = parseFloat($("#VI_C_4b_Int" + i).val()).toFixed(1);
                var cob = parseFloat($("#VI_C_4a_Cob" + i).val()).toFixed(1);
                var porc = (int / cob) * 100;
                var porc = parseFloat(porc).toFixed(1);
                $("#VI_C_4c_Por" + i).val(porc + "%");
            }

            i++;
        });

        $("#VI_C_4a_CobTotal").val(total);
        $("#VI_C_4a_CobTotal").change();
    });


    

    $(".intervenciones").change(function () {
        var total = 0;
        var i = 1;

        $(".intervenciones").each(function (index) {
            if (!isNaN($(this).val()) && $(this).val() != '') {
                total += parseInt($(this).val());
            }

            if (!isNaN($("#VI_C_4a_Cob" + i).val()) && $("#VI_C_4a_Cob" + i).val() != '' && !isNaN($("#VI_C_4b_Int" + i).val()) && $("#VI_C_4b_Int" + i).val() != '') {
                var int = parseFloat($("#VI_C_4b_Int" + i).val()).toFixed(1);
                var cob = parseFloat($("#VI_C_4a_Cob" + i).val()).toFixed(1);
                var porc = (int / cob) * 100;
                var porc = parseFloat(porc).toFixed(1);
                $("#VI_C_4c_Por" + i).val(porc + "%");
            }

            i++;
        });

        $("#VI_C_4b_IntTotal").val(total);
        $("#VI_C_4b_IntTotal").change();
    });


    $("#VI_C_4b_IntTotal").change(function () {
        if (!isNaN($("#VI_C_4b_IntTotal").val()) && $("#VI_C_4b_IntTotal").val() != '' && !isNaN($("#VI_C_4a_CobTotal").val()) && $("#VI_C_4a_CobTotal").val() != '') {
            var int = parseFloat($("#VI_C_4b_IntTotal").val()).toFixed(1);
            var cob = parseFloat($("#VI_C_4a_CobTotal").val()).toFixed(1);
            var porc = (int / cob) * 100;
            var porc = parseFloat(porc).toFixed(1);
            $("#VI_C_4c_PorTotal").val(porc + "%");
        }
    });

    $("#VI_C_4a_CobTotal").change(function () {
        if (!isNaN($("#VI_C_4b_IntTotal").val()) && $("#VI_C_4b_IntTotal").val() != '' && !isNaN($("#VI_C_4a_CobTotal").val()) && $("#VI_C_4a_CobTotal").val() != '') {
            var int = parseFloat($("#VI_C_4b_IntTotal").val()).toFixed(1);
            var cob = parseFloat($("#VI_C_4a_CobTotal").val()).toFixed(1);
            var porc = int / cob;
            var porc = parseFloat(porc).toFixed(1);
            $("#VI_C_4c_PorTotal").val(porc + "%");
        }

    });


    $(".auditaEgresos").change(function () {
        var total = 0;

        $(".auditaEgresos").each(function (index) {
            if (!isNaN($(this).val()) && $(this).val() != '') {
                total += parseInt($(this).val());
            }
        });

        $("#totalEgresos").val(total);
    });

    $(".auditaIngresos").change(function () {
        var total = 0;

        $(".auditaIngresos").each(function (index) {
            if (!isNaN($(this).val()) && $(this).val() != '') {
                total += parseInt($(this).val());
            }
        });

        $("#totalIngresos").val(total);
    });

    $(".auditaReintegros").change(function () {
        var total = 0;

        $(".auditaReintegros").each(function (index) {
            if (!isNaN($(this).val()) && $(this).val() != '') {
                total += parseInt($(this).val());
            }
        });

        $("#totalReintegros").val(total);
    });

    $(".auditaMIngresos").change(function () {
        var total = 0;

        $(".auditaMIngresos").each(function (index) {
            if (!isNaN($(this).val()) && $(this).val() != '') {
                total += parseInt($(this).val());
            }
        });

        $("#totalIngresosAuditados").val(total);
    });

    $(".auditaMEgresos").change(function () {
        var total = 0;

        $(".auditaMEgresos").each(function (index) {
            if (!isNaN($(this).val()) && $(this).val() != '') {
                total += parseInt($(this).val());
            }
        });

        $("#totalEgresosAuditados").val(total);
    });

    $(".auditaMEgresos").change();
    $(".auditaMIngresos").change();
    $(".auditaReintegros").change();
    $(".auditaIngresos").change();
    $(".auditaEgresos").change();

    $("#PeriodoDesde").change(function () {
        var periodoDesde = parseInt($("#PeriodoDesde").val());
        if (periodoDesde > parseInt($("#PeriodoHasta").val())) {
            alert("Debe seleccionar un período válido.");
            $("#PeriodoDesde").val($("#PeriodoHasta").val());
            return false;
        }
    });

    $("#PeriodoHasta").change(function () {
        var periodoHasta = parseInt($("#PeriodoHasta").val());
        if (periodoHasta < parseInt($("#PeriodoDesde").val())) {
            alert("Debe seleccionar un período válido.");
            $("#PeriodoHasta").val($("#PeriodoDesde").val());
            return false;
        }
    });

    $("#MesDesde").change(function () {
        var mesDesde = parseInt($("#MesDesde").val());
        if (parseInt($("#PeriodoHasta").val()) == parseInt($("#PeriodoDesde").val())) {
            // Si es el mismo periodo (año)
            if (mesDesde > parseInt($("#MesHasta").val())) {
                alert("Debe seleccionar un mes válido.");
                $("#MesDesde").val($("#MesHasta").val());
                return false;
            }
        }
    });

    $("#MesHasta").change(function () {
        var mesHasta = parseInt($("#MesHasta").val());
        if (parseInt($("#PeriodoHasta").val()) == parseInt($("#PeriodoDesde").val())) {
            // Si es el mismo periodo (año)
            if (mesHasta < parseInt($("#MesDesde").val())) {
                alert("Debe seleccionar un mes válido.");
                $("#MesHasta").val($("#MesDesde").val());
                return false;
            }
        }
    }); 

    //Indicadores de Gestion

    $(".PeriodoConfiguracionIndicadores").change(function(){
        var periodo = $(this).val();
        window.location = "/SAG_5/Control/Estandares/?Periodo=" + periodo;
    });

    $(".PeriodoIndicadores").change(function(){
        var periodo = $(this).val();
        window.location = "/SAG_5/Control/Indicadores/?Periodo=" + periodo;
    });

    /*
    $("#fixedHeader_Conciliacion").chromatable({
		width: "785px",
		height: "300px",
		scrolling: "yes"
	});	
    */
    // FUNCIONES UTILES

    // Imprimir pagina
    $("#imprimirDirecto").click(function () {
        window.print();
        return false;
    });

    $(".imprimirDirecto").click(function () {
        window.print();
        return false;
    });

    $(".imprimirDirectoAuditoria").click(function () {
        if ($(this).attr("programaID") == undefined) {
            window.print();
            return false;
        } else {
            var programa = $(this).attr("programaID");
            window.open("/SAG_5/Auditorias/ImprimirInforme?Programa=" + programa, 'Imprimir Informe Auditoría', '');
        }
    });

    $(".validarCuenta").click(function(){
        if ($("#CuentaID").val() == "0"){
            $('body').css('opacity','0.25');
            alert("Debe seleccionar Cuenta");
            $('body').css('opacity','1');
            $("#CuentaID").focus();
            return false;
        }
    });


    // Administracion Correlativos

    $(".proyecto_cerrar").click(function(){
        if (confirm("Se cerrara el proyecto y solo se podra revisar en forma de solo lectura, esta seguro?")) {
            var proyectoID = $(this).attr("proyecto");
            window.location = "./CerrarProyecto/" + proyectoID;
        }
        return false;
    });

    $(".proyecto_eliminar").click(function(){
        if (confirm("Se eliminara el proyecto y se borraran todos los datos asociados a el, esta seguro?")) {
            var proyectoID = $(this).attr("proyecto");
            window.location = "./EliminarProyecto/" + proyectoID;
        }
        return false;
    });

    $(".proyecto_saldo").click(function(){
        if (confirm("Se asignara el saldo inicial al periodo activo del proyecto. Los movimientos no se veran afectado, esta seguro?")) {
            var proyectoID = $(this).attr("proyecto");
            var monto = $(".monto_"+proyectoID).val();

            window.location = "./ReiniciarSaldo?proyectoID=" + proyectoID + "&monto=" + monto;
            return false;
        }
        return false;
    });

    $(".proyecto_reiniciar").click(function(){
        if (confirm("Se reiniciara el proyecto y se borraran todos los datos asociados a el, esta seguro?")) {
            var proyectoID = $(this).attr("proyecto");
            var ingreso = $(".ingreso_"+proyectoID).val();
            var egreso = $(".egreso_"+proyectoID).val();
            var reintegro = $(".reintegro_"+proyectoID).val();
            var deuda = $(".deuda_"+proyectoID).val();
            var monto = $(".monto_"+proyectoID).val();

            window.location = "./ReiniciarProyecto?proyectoID=" + proyectoID+"&ingreso="+ingreso+"&egreso="+egreso+"&reintegro="+reintegro+"&deuda="+deuda+"&monto="+monto;
        }
        return false;
    });

    // INVENTARIO
    $("#Inventario_Generar_Alta").click(function(){
        window.location = "./Alta";
        return false;
    });

    $("#Inventario_Generar_Baja").click(function(){
        window.location = "./Baja";
        return false;
    });

    $(".Inventario_Egreso").click(function(){
        window.open("/SAG_5/Inventario/ListadoDetalles/", 'Egresos', 'width=800,height=400');
        return false;
    });

    $(".guardarFila").click(function(){
        var fila = $(this).attr("fila");
        alert(fila);
    });

    $(".ingresar_persona_popup").click(function(){
        if ($("#Rut").val()=="" || $("#Rut").val()=="")
        {
            alert("Debe ingresar el rut");
            $("#Rut").focus();
            return false;
        }

        if ($("#Nombres").val()=="")
        {
            alert("Debe ingresar el nombre");
            $("#Nombres").focus();
            return false;
        }

        if ($("#ApellidoParterno").val()=="")
        {
            alert("Debe ingresar el apellido paterno");
            $("#ApellidoParterno").focus();
            return false;
        }
    });

    $("input[type='text']").change(function(e){
        $(this).val($(this).val().toUpperCase());
    });

  

 

    $(".guardar-boleta-pop-up").click(function(){
        if ($("#Concepto").val() == "") {
            alert("Debe ingresar un concepto.");
            $("#Concepto").focus();
            return false;
        }
        return true;
    });


    // Efecto de los avisos
    //$(".aviso").pulse({opacity: 0.8}, {duration : 100, pulses : 5});

    // Hoja de inventarios
    $("#ItemID").change(function(){
        //alert($(this).val());
        var disponible = $("#ItemID option:selected").attr("disponible")
        //var disponible = $(this).attr("disponible");
        //alert(disponible);
        $("#Cantidad").val(disponible);
        $("#Cantidad").focus();
    });
    
    $("#Cantidad").change(function(){
        var disponible = $("#ItemID option:selected").attr("disponible")
        
        if (parseInt($(this).val()) > parseInt(disponible)) {
            if (disponible == "1") {
                alert("Solo hay una unidad disponible.");
            } else {
                alert("Solo hay " + disponible + " unidades disponibles.");
            }
            $("#Cantidad").val(disponible);
            return false;
        }
    });


    $(".Autorizar").click(function () {
        var respuesta = prompt("Ingrese un comentario", "");
        if (respuesta != null) {
            var url = $(this).attr("href");
            url = url + "&comentario=" + respuesta;
            window.location = url;
        } else {
            alert("Debe ingresar un comentario");
            return false;
        }
    });

    // Validacion telefono y fax
    $("#Fono").change(function () {
        if (isNaN($("#Fono").val())) {
            alert("Debe ingresar un teléfono válido, sin espacios ni letras.");
            $("#Fono").focus();
        }
    });

    $("#Fax").change(function () {
        if (isNaN($("#Fax").val())) {
            alert("Debe ingresar un fax válido, sin espacios ni letras.");
            $("#Fax").focus();
        }
    });

    $(".validarCero").change(function () {
        var monto = $(this).val();
        if (monto != "") {
            if (monto > 0) {
                return true;
            }
        }
        alert("Debe ingresar un monto mayor a cero.");
        $(this).val("");
        $(this).focus();
    });

    $(".validarDocumento").change(function () {
        var monto = $(this).val();
        if (monto != "") {
            if (monto > 0) {
                return true;
            }
        }
        alert("Debe ingresar un número de documento mayor a cero.");
        $(this).val("");
        $(this).focus();
    });
   
    function addPeriod(nStr)
    {
        nStr += '';
        x = nStr.split('.');
        x1 = x[0];
        x2 = x.length > 1 ? '.' + x[1] : '';
        var rgx = /(\d+)(\d{3})/;
        while (rgx.test(x1)) {
            x1 = x1.replace(rgx, '$1' + '.' + '$2');
        }
        return x1 + x2;
    }
});