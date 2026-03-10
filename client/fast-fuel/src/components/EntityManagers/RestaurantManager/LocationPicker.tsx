import { useEffect, useState, useCallback } from 'react';
import { MapContainer, TileLayer, Marker, useMapEvents } from 'react-leaflet';
import L from 'leaflet';
import 'leaflet/dist/leaflet.css';

export type LocationPickerProps = {
  lat: number;
  lng: number;
  onLocationChange: (data: { lat: number; lng: number; address: string | null }) => void;
};

const markerIcon = L.icon({
  iconUrl: 'https://unpkg.com/leaflet@1.9.4/dist/images/marker-icon.png',
  iconRetinaUrl: 'https://unpkg.com/leaflet@1.9.4/dist/images/marker-icon-2x.png',
  shadowUrl: 'https://unpkg.com/leaflet@1.9.4/dist/images/marker-shadow.png',
  iconSize: [25, 41],
  iconAnchor: [12, 41],
});

export function LocationPicker({ lat, lng, onLocationChange }: LocationPickerProps) {
  const [position, setPosition] = useState<[number, number]>([lat, lng]);
  const [markerInstance, setMarkerInstance] = useState<L.Marker | null>(null);

  useEffect(() => {
    setPosition([lat, lng]);
    onLocationChange({ lat, lng, address: null });
  }, [lat, lng, onLocationChange]);

  const reverseGeocode = useCallback(async (latArg: number, lonArg: number) => {
    try {
      const url = `https://nominatim.openstreetmap.org/reverse?format=jsonv2&lat=${encodeURIComponent(
        latArg,
      )}&lon=${encodeURIComponent(lonArg)}&addressdetails=1`;

      const response = await fetch(url);
      if (!response.ok) {
        console.error('Reverse geocoding failed:', response.statusText);
        return null;
      }

      const data = await response.json();

      const address = data.address || {};
      const houseNumber = address.house_number;
      const road = address.road;
      const city = address.city || address.town || address.village;
      const state = address.state;
      const country = address.country;
      const postcode = address.postcode;

      const components = [houseNumber, road, city, state, country, postcode].filter(Boolean);

      if (components.length) return components.join(', ');
      if (data.display_name) return data.display_name;
      return null;
    } catch (error) {
      console.error('Error during reverse geocoding:', error);
      return null;
    }
  }, []);

  const handleMove = useCallback(
    async (newPos: [number, number]) => {
      setPosition(newPos);
      const address = await reverseGeocode(newPos[0], newPos[1]);
      onLocationChange({ lat: newPos[0], lng: newPos[1], address });
    },
    [onLocationChange, reverseGeocode],
  );

  function MapEvents() {
    useMapEvents({
      click(e: L.LeafletMouseEvent) {
        handleMove([e.latlng.lat, e.latlng.lng]).then();
      },
    });
    return null;
  }

  // attach dragend via markerInstance state to avoid unstable ref dependencies
  useEffect(() => {
    const marker = markerInstance;
    if (!marker) return;
    const onDragEnd = async () => {
      const latlng = marker.getLatLng();
      await handleMove([latlng.lat, latlng.lng]);
    };
    marker.on('dragend', onDragEnd);
    return () => {
      marker.off('dragend', onDragEnd);
    };
  }, [markerInstance, handleMove]);

  return (
    <div style={{ height: 320 }}>
      <MapContainer center={position} zoom={13} style={{ height: '100%', width: '100%' }}>
        <TileLayer
          attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
          url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
        />
        <MapEvents />
        <Marker
          position={position}
          icon={markerIcon}
          draggable
          ref={(ref) => {
            setMarkerInstance(ref as L.Marker | null);
          }}
        />
      </MapContainer>
    </div>
  );
}
