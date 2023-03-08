using System;
using System.Collections.Generic;
using System.Linq;
//using MathNet.Numerics.LinearAlgebra.Double;
//using MathLib;
//using LinqStatistics;
//using BaseTestLib;

namespace Utility
{

    public class IthComputeClass
    {

        protected static List<double> computeFitError(List<double> c, List<double> arL, List<double> arI)
        {
            List<double> fitError = new List<double>();

            double dFitL = 0.0;

            for (int n = 0; n < arL.Count; n++)
            {
                dFitL = c[0] + c[1] * arI[n];
                fitError.Add(dFitL - arL[n]);
            }

            return fitError;
        }


        public static bool Calc_fit_rsq(List<double> x, List<double> y, ref List<double> c1s, ref List<double> rsqs, ref List<double> rsqDiffs, double dXmin = 5.0)
        {
            if (x.Count < 2 || y.Count < 2 || x.Count != y.Count)
            {
                return false;
            }
            List<double> x_t;
            List<double> y_t;
            List<double> retRsq = new List<double>();
            List<double> dRsqDiff = new List<double>();
            List<double> dC1s = new List<double>();
            for (int j = 0; j < x.Count; ++j)
            {
                if (x[j] <= dXmin || j < 2)
                {
                    dC1s.Add(0.0);
                    retRsq.Add(1.0);
                    dRsqDiff.Add(0.0);
                    continue;
                }
                // array x contains DAC
                // array y contains L
                // for CAPI DAC between 15 to 50, larger window size is used for more accurate Rsq
                // note that 15 is the value of dIBiasMin that is used for calculating DC offset and used for filtering DAC/L arrays
                //if (x[j] < 15 || x[j] > 50)
                //{
                x_t = x.GetRange(0, j + 1);
                y_t = y.GetRange(0, j + 1);
                //}
                //else
                //{
                //    x_t = x.GetRange(0, j+10);
                //    y_t = y.GetRange(0, j+10);
                //}
                List<double> c = CPolynRegressionSolver.solve(1, x_t, y_t);
                double tempC = (y[j] - y[j - 1]) / (x[j] - x[j - 1]);
                dC1s.Add(tempC);
                double SSresid = 0.0;
                double yfit = 0.0;
                double dFitError = 0.0;
                double dYavg = y_t.Average();
                double dSStotal = 0.0;
                for (int i = 0; i < x_t.Count; ++i)
                {
                    yfit = x_t[i] * c[1] + c[0];
                    dFitError = y_t[i] - yfit;
                    SSresid += dFitError * dFitError;
                    dSStotal += (y_t[i] - dYavg) * (y_t[i] - dYavg);
                }
                //chenhuan
                // if dSStotal = 0, it will create NaN for rsq, so force it to 1 to keep rsq=1
                if (dSStotal == 0) dSStotal = 1;

                double rsq = 1 - SSresid / dSStotal;
                dRsqDiff.Add(rsq - retRsq.Last());
                retRsq.Add(rsq);
            }
            rsqDiffs = dRsqDiff;
            rsqs = retRsq;
            c1s = dC1s;
            return true;
        }

        //chenhuan
        //minPDSum was 10 but will make the if (x_t.Count < 2) TRUE very quickly and quit the Ith calculation
        // set minPDSum to 0 now
        public static bool IthCompute(List<CLIdata> arData, ref double Ith, ref double slope, ref double IBiasOf0dbm, ref double StdvError, ref double MaxError, double dIBiasMin = 15.0, double dIBiasMax = 110.0, double minPDSum = 0, int nUpperLimitDAC = 40, int NumPointsBeyondIth=-1)
        {
            // First, find which index correponds to 5 uW of Tx power(WLT: 5mA of PD current).
            // Data beyond this can be excluded

            //chenhuan
            //remove upperDAC which is supposed to be used in DC PBLI
            //var filterUpperDAC = (from mydata in arData where mydata.Tx_Bias_DDM >= dIBiasMin select mydata.DAC).ToList<double>();
            //var filterUpperDAC = (from mydata in arData where mydata.PD_SUM >= minPDSum select mydata.DAC).ToList<double>();

            //if (filterUpperDAC.Count <= 1)
            //{
            //    log("IthCompute cannot continue since there is no IBias is greatern than {0}A", dIBiasMin);
            //    return false;
            //}
            //else
            if (arData.Count > 1)
            {
                //check if there is DC offset for the laser
                var initialL = (from mydata in arData where mydata.DAC <= dIBiasMin select mydata.PD_SUM).ToList<double>();
                double dDCOffset = initialL.Min();

                //if (dDCOffset == 0.0)
                //{
                //    Ith = -999.0;
                //    slope = 0.0;
                //    IBiasOf0dbm = -999.0;
                //    return false;
                //}


                // apply filter to remove the samples below dIBiasMin, these samples only used yo determine DC Offset
                // and remove DC offset: i.e., select (mydata.PD_SUM - dDCOffset)
                var filterL = (from mydata in arData where (mydata.DAC > dIBiasMin && mydata.DAC <= dIBiasMax) select (mydata.PD_SUM - dDCOffset)).ToList<double>();
                var filterDAC = (from mydata in arData where (mydata.DAC > dIBiasMin && mydata.DAC <= dIBiasMax) select mydata.DAC).ToList<double>();


                List<double> rsqs = new List<double>();
                List<double> rsqDiffs = new List<double>();
                List<double> dC1s = new List<double>();
                if (filterDAC.Count != filterL.Count)
                {
                    //log("Number of L value: {0} and number of Tx_Bias_DDM: {1} is not the same", filterL.Count, filterDAC.Count);
                    Ith = -999.0;
                    return false;
                }

                //chenhuan
                // special case check: check if all filterL (the laser value array) are all 0's
                double dfilterLsum = 0.0;
                for (int j = 0; j < filterL.Count; ++j)
                    dfilterLsum += filterL[j];
                if (dfilterLsum == 0)
                {
                    //log("All PD Sums are zeros, quitting Ith Compute");
                    Ith = -999.0;
                    return false;
                }

                bool bRet = Calc_fit_rsq(filterDAC, filterL, ref dC1s, ref rsqs, ref rsqDiffs);
                //chenhuan
                // check bRet: it is false for a case that PD values are way too large >=4000  and filterL/filterDAC is zero length
                // if continue then will have "out of range" error when accessing a zero-length array 
                if (bRet == false)
                {
                    Ith = -999.0;
                    return false;
                }

                double minRsq = rsqs.Min();

                int minRsqIdx = rsqs.FindIndex(
                    delegate(double rsq)
                    {
                        return (rsq == minRsq);
                    }
                );

                // chenhuan
                // minPDSum is the PDSum of the element with minRsq
                // if minRsq is < 0.05, it will not enter the following WHILE loop
                // up to now, minPDSum is a constant estimation value
                // so update minPDSum now
                // since we remove DCOffset, add it back when comparing with the PDSum in myData
                minPDSum = filterL[minRsqIdx] + dDCOffset;



                //if the data is really noisy, find the outlier with minRsq and remove it until minRsq is >=0.05
                // chenhun
                //below was if; changed to while and include more instruction inside the while loop to filter out noisy data
                //if (minRsq < 0.05)
                //while (minRsq < 0.05)
                // since we know that Ith is around 40-50 DAC, so anything < 20 DAC is considered as noise
                while (minRsq < 0.05 || filterDAC[minRsqIdx] < 20)
                {
                    //chenhuan
                    // was >; now >=; othewise the while loop may enter endless since the worst noisy data is not filtered out
                    //if (filterL[minRsqIdx] > minPDSum)
                    //if (filterL[minRsqIdx] >= minPDSum)
                    //{
                    //    dPDsum = 0.0;
                    //    for (int i = 0; i < (minRsqIdx + 1); ++i)
                    //        dPDsum += filterL[i];
                    //    minPDSum = dPDsum / minRsqIdx;
                    //}
                    // instead of the above loop, just remove the noisy sample with minRsq by setting the minPDSum and the filter below
                    minPDSum = filterL[minRsqIdx] + dDCOffset;

                    //chenhuan
                    //}
                    // to put the following as a part of if (minRsq < 0.05)
                    filterL = (from mydata in arData where (mydata.PD_SUM > minPDSum && mydata.DAC <= dIBiasMax) select (mydata.PD_SUM - dDCOffset)).ToList<double>();
                    filterDAC = (from mydata in arData where (mydata.PD_SUM > minPDSum && mydata.DAC <= dIBiasMax) select mydata.DAC).ToList<double>();
                    bRet = Calc_fit_rsq(filterDAC, filterL, ref dC1s, ref rsqs, ref rsqDiffs);
                    //chenhuan
                    // check bRet: it is false for a case that PD values are way too large >=4000  and filterL/filterDAC is zero length
                    // if continue then will have "out of range" error when accessing a zero-length arrary 
                    if (bRet == false)
                    {
                        Ith = -999.0;
                        return false;
                    }

                    minRsq = rsqs.Min();


                    minRsqIdx = rsqs.FindIndex(
                         delegate(double rsq)
                         {
                             return (rsq == minRsq);
                         }
                    );
                }// new place for the end of while (minRsq < 0.05)



                // find the overall min value of rsqs and its index (largest valley)
                const double RsqBound = 0.9; // value was 0.9 for PSM4
                int nRsq90 = 0;

                // Find the first droop of Rsq
                int n1stRsqDropIdx = 0;
                n1stRsqDropIdx = rsqDiffs.FindIndex(
                     delegate(double rsqDif)
                     {
                         return (rsqDif < 0.0);
                     }
                );
                // Locate the onset of the largest valley
                int thresh_i_rsq = 0;
                double thresh_rsq = 0.0;
                if (n1stRsqDropIdx < minRsqIdx)
                {
                    int nNextMaxRsq = rsqDiffs.Skip(n1stRsqDropIdx + 1).ToList<double>().FindIndex(
                      delegate(double rsqDif)
                      {
                          return rsqDif < 0.0;
                      }
                    ) + n1stRsqDropIdx;

                    //chenhuan
                    // check if nNextMaxRsq is within range: one case where PD values are large, results in negative nNextMaxRsq
                    if (nNextMaxRsq < 0 || nNextMaxRsq > rsqs.Count)
                    {
                        Ith = -999.0;
                        return false;
                    }

                    nRsq90 = 0;
                    if (rsqs[nNextMaxRsq] > RsqBound)
                    {

                        nRsq90 = rsqs.Skip(nNextMaxRsq + 1).ToList<double>().FindIndex(
                         delegate(double rsq)
                         {
                             return rsq <= RsqBound;
                         }
                      );

                    }
                    else
                    {
                        nRsq90 = rsqs.Skip(nNextMaxRsq + 1).ToList<double>().FindIndex(
                         delegate(double rsq)
                         {
                             return rsq >= RsqBound;
                         }
                      );
                    }
                    if (nRsq90 > 0)
                    {
                        nRsq90 += nNextMaxRsq + 1;
                    }
                }
                else
                {
                    int nNextMaxRsq = rsqDiffs.Skip(minRsqIdx + 1).ToList<double>().FindIndex(
                   delegate(double rsqDif)
                   {
                       return rsqDif < 0.0;
                   }
                ) + minRsqIdx + 1;
                    //if there is no dip find the rsq greater than RsqBound as onset
                    nRsq90 = 0;
                    if (nNextMaxRsq <= (minRsqIdx + 1))
                    {
                        nRsq90 = rsqDiffs.Skip(minRsqIdx + 1).ToList<double>().FindIndex(
                            delegate(double rsq)
                            {
                                return rsq >= RsqBound;
                            }
                        );
                        if (nRsq90 > 0)
                        {
                            nRsq90 += minRsqIdx + 1;
                        }
                    }
                    else
                    {

                        nRsq90 = rsqs.Skip(nNextMaxRsq).ToList<double>().FindIndex(
                         delegate(double rsq)
                         {
                             return rsq <= RsqBound;
                         }
                       );
                        if (nRsq90 > 0)
                        {
                            nRsq90 += nNextMaxRsq + 1;
                        }
                    }
                }

                // if there is no Rsq > RsqBound
                if (nRsq90 <= 0)
                {
                    thresh_i_rsq = minRsqIdx;

                }
                else
                {
                    thresh_i_rsq = nRsq90;
                }

                // the following is linear regression for line fitting from the threshold point for 10 points to caculate slope, StdvError and MaxError
                // now based on NumPointsBeyondIth=-1 or not, calculate the slope, from NumPointsBeyondIth points beyond Ith, all the way to the end of sample points
                thresh_rsq = filterL[minRsqIdx];
                List<double> x_t = new List<double>(10);
                List<double> y_t = new List<double>(10);
                ////for (int i = thresh_i_rsq; i < ((thresh_i_rsq + 10 < filterDAC.Count) ? thresh_i_rsq + 10 : filterDAC.Count); ++i)
                //// chenhuan 
                ////added i>=0 to the following loop condition; otherwise for a x_t/y_t with less than 10 elements, i will become negative when i--
                ////for (int i = thresh_i_rsq; i < ((thresh_i_rsq + 10 < filterDAC.Count) ? thresh_i_rsq + 10 : filterDAC.Count); --i)
                //// changed from --i to ++i because we want to find the slope of the line starting from thresh_i_rsq towards rising LI curve
                if (NumPointsBeyondIth == -1) // use NumPointsBeyondIth to decide to use 10 points or all points
                {
                    for (int i = thresh_i_rsq; (i >= 0 && i < ((thresh_i_rsq + 10 < filterDAC.Count) ? thresh_i_rsq + 10 : filterDAC.Count)); ++i)
                    {
                        x_t.Add(filterDAC[i]);
                        y_t.Add(filterL[i]);
                    }
                }
                else
                {
                    for (int i = (thresh_i_rsq + NumPointsBeyondIth); i < filterDAC.Count; ++i)
                    {
                        x_t.Add(filterDAC[i]);
                        y_t.Add(filterL[i]);
                    }
                }
                if (x_t.Count < 2)
                {
                    //log("Number of data point {0} is not enough to compute the slope", x_t.Count);
                    Ith = -999.0;
                    return false;
                }
                List<double> c = CPolynRegressionSolver.solve(1, x_t, y_t);
                Ith = (minPDSum - 1.0 * c[0]) / c[1];
                slope = c[1];
                // y = c[1]x + c[0]
                IBiasOf0dbm = (1.0 - c[0]) / c[1];
                //Now compute the stats to determine the goodness of the fit...
                List<double> arFitError = computeFitError(c, filterL, filterDAC);
                StdvError = arFitError.StandardDeviation();
                var arMaxAbsError = from x in arFitError select Math.Abs(x);
                MaxError = arMaxAbsError.Max();
            }
            return true;
        }

    }
}
