﻿var ProgrammerViewModel = function () {
    var self = this;

    self.deviceName = ko.observable();
    self.useDHCP = ko.observable(false);
    self.staticIp = ko.observable();
    self.gateway = ko.observable();
    self.netmask = ko.observable();
    self.dns = ko.observable();

    self.programDevice = function () {
        $.ajax({
            type: 'POST',
            url: 'api/Programmer',
            data: { deviceName: self.deviceName },
            dataType: 'json'
        }).done(function (msg) {
            console.log(msg);
        });
    };
}

var viewModel = new ProgrammerViewModel();



$(function () {
    //$("[name='useDHCPf']").bootstrapSwitch();

    ko.bindingHandlers.bootstrapSwitch = {
        init: function (element, valueAccessor, allBindingsAccessor) {
            //initialize bootstrapSwitch
            $(element).bootstrapSwitch();

            // setting initial value
            $(element).bootstrapSwitch('state', valueAccessor()());

            //handle the field changing
            $(element).on('switchChange.bootstrapSwitch', function (event, state) {
                var observable = valueAccessor();
                observable(state);
            });

            // Adding component options
            var options = allBindingsAccessor().bootstrapSwitchOptions || {};
            for (var property in options) {
                $(element).bootstrapSwitch(property, ko.utils.unwrapObservable(options[property]));
            }

            //handle disposal (if KO removes by the template binding)
            ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
                $(element).bootstrapSwitch("destroy");
            });

        },
        //update the control when the view model changes
        update: function (element, valueAccessor, allBindingsAccessor) {
            var value = ko.utils.unwrapObservable(valueAccessor());

            // Adding component options
            var options = allBindingsAccessor().bootstrapSwitchOptions || {};
            for (var property in options) {
                $(element).bootstrapSwitch(property, ko.utils.unwrapObservable(options[property]));
            }

            $(element).bootstrapSwitch("state", value);
        }
    };


    ko.applyBindings(viewModel);

    
});

